using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;

namespace SchoolMS.Application.Services;

public class StudentSubscriptionService : IStudentSubscriptionService
{
    private readonly IRepository<StudentSubscription> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITeacherEarningService _earningService;

    public StudentSubscriptionService(
        IRepository<StudentSubscription> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITeacherEarningService earningService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _earningService = earningService;
    }

    public async Task<List<StudentSubscriptionDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(s => s.Student).ThenInclude(st => st.Branch)
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Include(s => s.School)
            .ToListAsync();
        return _mapper.Map<List<StudentSubscriptionDto>>(items);
    }

    public async Task<List<StudentSubscriptionDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(s => s.SchoolId == schoolId)
            .Include(s => s.Student).ThenInclude(st => st.Branch)
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Include(s => s.School)
            .ToListAsync();
        return _mapper.Map<List<StudentSubscriptionDto>>(items);
    }

    public async Task<List<StudentSubscriptionDto>> GetByStudentIdAsync(int studentId, int schoolId)
    {
        var items = await _repository.Query()
            .Where(s => s.StudentId == studentId && s.SchoolId == schoolId)
            .Include(s => s.Student).ThenInclude(st => st.Branch)
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Include(s => s.School)
            .ToListAsync();
        return _mapper.Map<List<StudentSubscriptionDto>>(items);
    }

    public async Task<StudentSubscriptionDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(s => s.Student).ThenInclude(st => st.Branch)
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Include(s => s.School)
            .FirstOrDefaultAsync(s => s.Id == id);
        return entity == null ? null : _mapper.Map<StudentSubscriptionDto>(entity);
    }

    public async Task<StudentSubscriptionDto> CreateAsync(StudentSubscriptionDto dto)
    {
        var entity = _mapper.Map<StudentSubscription>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentSubscriptionDto>(entity);
    }

    public async Task<StudentSubscriptionDto> UpdateAsync(StudentSubscriptionDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"StudentSubscription with ID {dto.Id} not found.");
        entity.StudentId = dto.StudentId;
        entity.OnlineSubscriptionPlanId = dto.OnlineSubscriptionPlanId;
        entity.Status = dto.Status;
        entity.PaidAmount = dto.PaidAmount;
        entity.PromoCode = dto.PromoCode;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentSubscriptionDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"StudentSubscription with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, SubscriptionStatus status)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"StudentSubscription with ID {id} not found.");
        entity.Status = status;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        // تسجيل أرباح المدرسين تلقائياً عند الموافقة
        if (status == SubscriptionStatus.Approved)
            await _earningService.RecordEarningForSubscriptionAsync(id);
    }

    public async Task MarkAsPaidAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(s => s.OnlineSubscriptionPlan)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"StudentSubscription with ID {id} not found.");
        entity.PaidAmount = entity.OriginalAmount - entity.DiscountAmount;
        entity.Status = SubscriptionStatus.Approved;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        // تسجيل أرباح المدرسين تلقائياً عند تأكيد الدفع
        await _earningService.RecordEarningForSubscriptionAsync(id);
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("StudentSubscriptions");
        var ws = workbook.Worksheet("StudentSubscriptions");
        ws.Cell(1, 1).Value = "Student";
        ws.Cell(1, 2).Value = "Plan";
        ws.Cell(1, 3).Value = "Type";
        ws.Cell(1, 4).Value = "Subject";
        ws.Cell(1, 5).Value = "Status";
        ws.Cell(1, 6).Value = "Original Amount";
        ws.Cell(1, 7).Value = "Discount";
        ws.Cell(1, 8).Value = "Paid Amount";
        ws.Cell(1, 9).Value = "Promo Code";
        ws.Cell(1, 10).Value = "Start Date";
        ws.Cell(1, 11).Value = "End Date";
        ws.Cell(1, 12).Value = "Branch";
        ws.Cell(1, 13).Value = "School";
        ws.Range("A1:M1").Style.Font.Bold = true;
        ws.Range("A1:M1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:M1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].StudentName;
            ws.Cell(i + 2, 2).Value = items[i].PlanName;
            ws.Cell(i + 2, 3).Value = items[i].SubscriptionType.ToString();
            ws.Cell(i + 2, 4).Value = items[i].SubjectName;
            ws.Cell(i + 2, 5).Value = items[i].Status.ToString();
            ws.Cell(i + 2, 6).Value = items[i].OriginalAmount;
            ws.Cell(i + 2, 7).Value = items[i].DiscountAmount;
            ws.Cell(i + 2, 8).Value = items[i].PaidAmount;
            ws.Cell(i + 2, 9).Value = items[i].PromoCode;
            ws.Cell(i + 2, 10).Value = items[i].StartDate.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 11).Value = items[i].EndDate.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 12).Value = items[i].BranchName;
            ws.Cell(i + 2, 13).Value = items[i].SchoolName;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
