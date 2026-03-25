using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class TeacherEarningService : ITeacherEarningService
{
    private readonly IRepository<TeacherEarning> _repository;
    private readonly IRepository<StudentSubscription> _subscriptionRepo;
    private readonly IRepository<Course> _courseRepo;
    private readonly IRepository<School> _schoolRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherEarningService(
        IRepository<TeacherEarning> repository,
        IRepository<StudentSubscription> subscriptionRepo,
        IRepository<Course> courseRepo,
        IRepository<School> schoolRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _subscriptionRepo = subscriptionRepo;
        _courseRepo = courseRepo;
        _schoolRepo = schoolRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IQueryable<TeacherEarning> IncludeAll() =>
        _repository.Query()
            .Include(e => e.Teacher)
            .Include(e => e.Course)
            .Include(e => e.StudentSubscription).ThenInclude(s => s!.Student)
            .Include(e => e.StudentSubscription).ThenInclude(s => s!.OnlineSubscriptionPlan)
            .Include(e => e.School);

    public async Task<List<TeacherEarningDto>> GetAllAsync()
    {
        var items = await IncludeAll().ToListAsync();
        return _mapper.Map<List<TeacherEarningDto>>(items);
    }

    public async Task<List<TeacherEarningDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await IncludeAll()
            .Where(e => e.SchoolId == schoolId)
            .ToListAsync();
        return _mapper.Map<List<TeacherEarningDto>>(items);
    }

    public async Task<List<TeacherEarningDto>> GetByTeacherIdAsync(int teacherId, int schoolId)
    {
        var items = await IncludeAll()
            .Where(e => e.TeacherId == teacherId && e.SchoolId == schoolId)
            .ToListAsync();
        return _mapper.Map<List<TeacherEarningDto>>(items);
    }

    public async Task<TeacherEarningDto?> GetByIdAsync(int id)
    {
        var entity = await IncludeAll().FirstOrDefaultAsync(e => e.Id == id);
        return entity == null ? null : _mapper.Map<TeacherEarningDto>(entity);
    }

    /// <summary>
    /// يتم استدعاؤها تلقائياً عند الموافقة على اشتراك طالب.
    /// تبحث عن الكورسات المرتبطة بمادة خطة الاشتراك وتسجّل أرباح المدرسين تلقائياً.
    /// </summary>
    public async Task RecordEarningForSubscriptionAsync(int studentSubscriptionId)
    {
        // تحقق من عدم وجود أرباح مسجلة مسبقاً لهذا الاشتراك
        var alreadyExists = await _repository.AnyAsync(e => e.StudentSubscriptionId == studentSubscriptionId);
        if (alreadyExists) return;

        var subscription = await _subscriptionRepo.Query()
            .Include(s => s.OnlineSubscriptionPlan)
            .FirstOrDefaultAsync(s => s.Id == studentSubscriptionId);
        if (subscription == null) return;

        var paidAmount = subscription.PaidAmount > 0
            ? subscription.PaidAmount
            : subscription.OriginalAmount - subscription.DiscountAmount;
        if (paidAmount <= 0) return;

        // جلب إعدادات النسبة الافتراضية من المدرسة
        var school = await _schoolRepo.Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == subscription.SchoolId);
        var defaultRate = school?.DefaultTeacherCommissionRate ?? 0;

        // البحث عن الكورسات المنشورة المرتبطة بنفس المادة
        var courses = await _courseRepo.Query()
            .Where(c => c.SubjectId == subscription.OnlineSubscriptionPlan.SubjectId
                        && c.SchoolId == subscription.SchoolId
                        && c.IsPublished)
            .ToListAsync();

        if (courses.Count == 0) return;

        foreach (var course in courses)
        {
            var rate = course.CommissionRate ?? defaultRate;
            if (rate <= 0) continue;

            var earningAmount = paidAmount * rate / 100m;

            var entity = new TeacherEarning
            {
                TeacherId = course.TeacherId,
                CourseId = course.Id,
                StudentSubscriptionId = studentSubscriptionId,
                SubscriptionAmount = paidAmount,
                CommissionRate = rate,
                EarningAmount = earningAmount,
                Status = TeacherEarningStatus.Pending,
                SchoolId = subscription.SchoolId
            };
            await _repository.AddAsync(entity);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, TeacherEarningStatus status)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"TeacherEarning with ID {id} not found.");
        entity.Status = status;
        if (status == TeacherEarningStatus.Paid)
            entity.PaidAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAsPaidAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"TeacherEarning with ID {id} not found.");
        entity.Status = TeacherEarningStatus.Paid;
        entity.PaidAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"TeacherEarning with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<TeacherEarningSummaryDto>> GetSummaryBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(e => e.SchoolId == schoolId)
            .Include(e => e.Teacher)
            .ToListAsync();

        return items.GroupBy(e => new { e.TeacherId, e.Teacher.FullName })
            .Select(g => new TeacherEarningSummaryDto
            {
                TeacherId = g.Key.TeacherId,
                TeacherName = g.Key.FullName,
                TotalEarnings = g.Sum(x => x.EarningAmount),
                PendingEarnings = g.Where(x => x.Status == TeacherEarningStatus.Pending).Sum(x => x.EarningAmount),
                ApprovedEarnings = g.Where(x => x.Status == TeacherEarningStatus.Approved).Sum(x => x.EarningAmount),
                PaidEarnings = g.Where(x => x.Status == TeacherEarningStatus.Paid).Sum(x => x.EarningAmount),
                TotalTransactions = g.Count()
            }).ToList();
    }

    public async Task<TeacherEarningSummaryDto?> GetTeacherSummaryAsync(int teacherId, int schoolId)
    {
        var items = await _repository.Query()
            .Where(e => e.TeacherId == teacherId && e.SchoolId == schoolId)
            .Include(e => e.Teacher)
            .ToListAsync();

        if (items.Count == 0) return null;

        var teacher = items.First().Teacher;
        return new TeacherEarningSummaryDto
        {
            TeacherId = teacherId,
            TeacherName = teacher.FullName,
            TotalEarnings = items.Sum(x => x.EarningAmount),
            PendingEarnings = items.Where(x => x.Status == TeacherEarningStatus.Pending).Sum(x => x.EarningAmount),
            ApprovedEarnings = items.Where(x => x.Status == TeacherEarningStatus.Approved).Sum(x => x.EarningAmount),
            PaidEarnings = items.Where(x => x.Status == TeacherEarningStatus.Paid).Sum(x => x.EarningAmount),
            TotalTransactions = items.Count
        };
    }

    public async Task<byte[]> ExportToExcelAsync(int? schoolId = null, int? teacherId = null)
    {
        var query = IncludeAll();
        if (schoolId.HasValue) query = query.Where(e => e.SchoolId == schoolId.Value);
        if (teacherId.HasValue) query = query.Where(e => e.TeacherId == teacherId.Value);
        var items = _mapper.Map<List<TeacherEarningDto>>(await query.ToListAsync());

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("TeacherEarnings");
        var ws = workbook.Worksheet("TeacherEarnings");

        var headers = new[] { "Teacher", "Course", "Student", "Plan", "Subscription Amount", "Commission %", "Earning Amount", "Status", "Paid At", "Notes", "Date", "School" };
        for (int h = 0; h < headers.Length; h++)
        {
            ws.Cell(1, h + 1).Value = headers[h];
        }
        ws.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
        ws.Range(1, 1, 1, headers.Length).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range(1, 1, 1, headers.Length).Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].TeacherName;
            ws.Cell(i + 2, 2).Value = items[i].CourseTitle;
            ws.Cell(i + 2, 3).Value = items[i].StudentName;
            ws.Cell(i + 2, 4).Value = items[i].PlanName;
            ws.Cell(i + 2, 5).Value = items[i].SubscriptionAmount;
            ws.Cell(i + 2, 6).Value = items[i].CommissionRate;
            ws.Cell(i + 2, 7).Value = items[i].EarningAmount;
            ws.Cell(i + 2, 8).Value = items[i].Status.ToString();
            ws.Cell(i + 2, 9).Value = items[i].PaidAt?.ToString("yyyy-MM-dd") ?? "";
            ws.Cell(i + 2, 10).Value = items[i].Notes;
            ws.Cell(i + 2, 11).Value = items[i].CreatedAt.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 12).Value = items[i].SchoolName;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
