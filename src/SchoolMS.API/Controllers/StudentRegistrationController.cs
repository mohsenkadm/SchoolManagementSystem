using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class StudentRegistrationController : ControllerBase
{
    private readonly IOtpService _otpService;
    private readonly IStudentService _studentService;
    private readonly IRepository<Student> _studentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public StudentRegistrationController(IOtpService otpService, IStudentService studentService,
        IRepository<Student> studentRepo, IUnitOfWork unitOfWork)
    {
        _otpService = otpService;
        _studentService = studentService;
        _studentRepo = studentRepo;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Step 1: Send OTP code to student's phone via WhatsApp
    /// </summary>
    [HttpPost("send-otp")]
    public async Task<ActionResult<SendOtpResponseDto>> SendOtp([FromBody] SendOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest(new SendOtpResponseDto { Success = false, Message = "رقم الهاتف مطلوب." });

        request.Purpose = "StudentRegistration";
        var result = await _otpService.SendOtpAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Step 2: Verify the OTP code and receive a verification token
    /// </summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<VerifyOtpResponseDto>> VerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Code))
            return BadRequest(new VerifyOtpResponseDto { Success = false, Message = "رقم الهاتف وكود التحقق مطلوبان." });

        var result = await _otpService.VerifyOtpAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Step 3: Complete student registration using the verification token
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<StudentDto>> Register([FromBody] RegisterStudentWithOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.VerificationToken))
            return BadRequest(new { error = "رمز التحقق مطلوب." });

        // Validate that the OTP was verified for this phone
        var isValid = await _otpService.ValidateVerificationTokenAsync(dto.Phone, dto.VerificationToken);
        if (!isValid)
            return BadRequest(new { error = "رمز التحقق غير صالح أو منتهي الصلاحية. أعد التحقق من الرقم." });

        // Create the student
        var createDto = new CreateStudentDto
        {
            FullName = dto.FullName,
            FullNameAr = dto.FullNameAr,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            NationalId = dto.NationalId,
            Address = dto.Address,
            Phone = dto.Phone,
            ParentPhone = dto.ParentPhone,
            ParentName = dto.ParentName,
            Email = dto.Email,
            Username = dto.Username,
            Password = dto.Password,
            BranchId = dto.BranchId,
            ClassRoomId = dto.ClassRoomId,
            AcademicYearId = dto.AcademicYearId,
            Notes = dto.Notes
        };

        var student = await _studentService.CreateAsync(createDto);
        return Ok(student);
    }

    /// <summary>
    /// Step 1: Send OTP for forgot password
    /// </summary>
    [HttpPost("forgot-password/send-otp")]
    public async Task<ActionResult<SendOtpResponseDto>> ForgotPasswordSendOtp([FromBody] SendOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest(new SendOtpResponseDto { Success = false, Message = "رقم الهاتف مطلوب." });

        var studentExists = await _studentRepo.Query().AnyAsync(s => s.Phone == request.Phone);
        if (!studentExists)
            return BadRequest(new SendOtpResponseDto { Success = false, Message = "لا يوجد طالب مسجل بهذا الرقم." });

        request.Purpose = "ForgotPassword";
        var result = await _otpService.SendOtpAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Step 2: Verify OTP for forgot password
    /// </summary>
    [HttpPost("forgot-password/verify-otp")]
    public async Task<ActionResult<VerifyOtpResponseDto>> ForgotPasswordVerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Code))
            return BadRequest(new VerifyOtpResponseDto { Success = false, Message = "رقم الهاتف وكود التحقق مطلوبان." });

        var result = await _otpService.VerifyOtpAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Step 3: Reset password after OTP verification
    /// </summary>
    [HttpPost("forgot-password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        if (string.IsNullOrWhiteSpace(request.VerificationToken) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new { error = "رمز التحقق وكلمة المرور الجديدة مطلوبان." });

        var isValid = await _otpService.ValidateVerificationTokenAsync(request.Phone, request.VerificationToken);
        if (!isValid)
            return BadRequest(new { error = "رمز التحقق غير صالح أو منتهي الصلاحية." });

        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Phone == request.Phone);
        if (student == null)
            return BadRequest(new { error = "لا يوجد طالب مسجل بهذا الرقم." });

        student.Password = request.NewPassword;
        _studentRepo.Update(student);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "تم تغيير كلمة المرور بنجاح." });
    }
}
