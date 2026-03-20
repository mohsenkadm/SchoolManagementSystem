using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class OnlineSubscriptionPlanService : IOnlineSubscriptionPlanService
{
    private readonly IRepository<OnlineSubscriptionPlan> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OnlineSubscriptionPlanService(IRepository<OnlineSubscriptionPlan> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<OnlineSubscriptionPlanDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(p => p.Subject)
            .Include(p => p.School)
            .Include(p => p.StudentSubscriptions)
            .ToListAsync();
        return _mapper.Map<List<OnlineSubscriptionPlanDto>>(items);
    }

    public async Task<List<OnlineSubscriptionPlanDto>> GetBySchoolIdAsync(int schoolId, string? search = null)
    {
        var query = _repository.Query().Where(p => p.SchoolId == schoolId);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.PlanName.Contains(search));
        var items = await query
            .Include(p => p.Subject)
            .Include(p => p.School)
            .Include(p => p.StudentSubscriptions)
            .ToListAsync();
        return _mapper.Map<List<OnlineSubscriptionPlanDto>>(items);
    }

    public async Task<OnlineSubscriptionPlanDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(p => p.Subject)
            .Include(p => p.School)
            .Include(p => p.StudentSubscriptions)
            .FirstOrDefaultAsync(p => p.Id == id);
        return entity == null ? null : _mapper.Map<OnlineSubscriptionPlanDto>(entity);
    }

    public async Task<OnlineSubscriptionPlanDto> CreateAsync(OnlineSubscriptionPlanDto dto)
    {
        var entity = _mapper.Map<OnlineSubscriptionPlan>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OnlineSubscriptionPlanDto>(entity);
    }

    public async Task<OnlineSubscriptionPlanDto> UpdateAsync(OnlineSubscriptionPlanDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"OnlineSubscriptionPlan with ID {dto.Id} not found.");
        entity.PlanName = dto.PlanName;
        entity.Price = dto.Price;
        entity.DurationMonths = dto.DurationMonths;
        entity.SubjectId = dto.SubjectId;
        entity.SubscriptionType = dto.SubscriptionType;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OnlineSubscriptionPlanDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"OnlineSubscriptionPlan with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("OnlineSubscriptionPlans");
        var ws = workbook.Worksheet("OnlineSubscriptionPlans");
        ws.Cell(1, 1).Value = "Plan Name";
        ws.Cell(1, 2).Value = "Type";
        ws.Cell(1, 3).Value = "Price";
        ws.Cell(1, 4).Value = "Duration (Months)";
        ws.Cell(1, 5).Value = "Subject";
        ws.Cell(1, 6).Value = "School";
        ws.Cell(1, 7).Value = "Subscribers";
        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:G1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].PlanName;
            ws.Cell(i + 2, 2).Value = items[i].SubscriptionType.ToString();
            ws.Cell(i + 2, 3).Value = items[i].Price;
            ws.Cell(i + 2, 4).Value = items[i].DurationMonths;
            ws.Cell(i + 2, 5).Value = items[i].SubjectName;
            ws.Cell(i + 2, 6).Value = items[i].SchoolName;
            ws.Cell(i + 2, 7).Value = items[i].StudentSubscriptionCount;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
