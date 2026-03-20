using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة اشتراكات الطلاب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/student-subscriptions")]
[Authorize]
public class StudentSubscriptionsController : ControllerBase
{
    private readonly IStudentSubscriptionService _service;
    private readonly IOnlineSubscriptionPlanService _planService;
    private readonly IPromoCodeService _promoCodeService;
    private readonly IOneSignalNotificationService _pushService;

    public StudentSubscriptionsController(
        IStudentSubscriptionService service,
        IOnlineSubscriptionPlanService planService,
        IPromoCodeService promoCodeService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _planService = planService;
        _promoCodeService = promoCodeService;
        _pushService = pushService;
    }

    // جلب اشتراكات الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<StudentSubscriptionDto>>> GetStudentSubscriptions(int schoolId)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Student") return Forbid();
        var studentId = int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());
        return Ok(await _service.GetByStudentIdAsync(studentId, schoolId));
    }

    // اشتراك طالب في خطة جديدة
    [HttpPost("subscribe")]
    public async Task<ActionResult<StudentSubscriptionDto>> Subscribe(int schoolId, [FromBody] SubscribeRequestDto request)
    {
        var plan = await _planService.GetByIdAsync(request.OnlineSubscriptionPlanId);
        if (plan == null) return BadRequest(new { error = "Plan not found." });

        decimal originalAmount = plan.Price;
        decimal discountAmount = 0;
        string? promoCode = null;

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            var (valid, error, discount) = await _promoCodeService.ValidateAndCalculateDiscountAsync(
                request.PromoCode, request.StudentId, originalAmount);
            if (!valid) return BadRequest(new { error });
            discountAmount = discount;
            promoCode = request.PromoCode;
        }

        var dto = new StudentSubscriptionDto
        {
            StudentId = request.StudentId,
            OnlineSubscriptionPlanId = request.OnlineSubscriptionPlanId,
            Status = SubscriptionStatus.Pending,
            OriginalAmount = originalAmount,
            DiscountAmount = discountAmount,
            PaidAmount = originalAmount - discountAmount,
            PromoCode = promoCode,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(plan.DurationMonths),
            SchoolId = request.SchoolId > 0 ? request.SchoolId : plan.SchoolId
        };

        var created = await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("Subscription Created", "Your subscription has been created", request.StudentId, "Student", dto.SchoolId);

        if (!string.IsNullOrWhiteSpace(promoCode))
        {
            var promoEntity = await _promoCodeService.GetByCodeAsync(promoCode);
            if (promoEntity != null)
                await _promoCodeService.RecordUsageAsync(promoEntity.Id, request.StudentId, created.Id);
        }

        return Ok(created);
    }
            
}
