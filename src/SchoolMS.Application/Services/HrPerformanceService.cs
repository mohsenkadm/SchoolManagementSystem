using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrPerformanceService : IHrPerformanceService
{
    private readonly IRepository<HrPerformanceCycle> _cycleRepo;
    private readonly IRepository<HrPerformanceCriteria> _criteriaRepo;
    private readonly IRepository<HrPerformanceReview> _reviewRepo;
    private readonly IRepository<HrPerformanceScore> _scoreRepo;
    private readonly IRepository<HrKpi> _kpiRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrPerformanceService(IRepository<HrPerformanceCycle> cycleRepo, IRepository<HrPerformanceCriteria> criteriaRepo,
        IRepository<HrPerformanceReview> reviewRepo, IRepository<HrPerformanceScore> scoreRepo,
        IRepository<HrKpi> kpiRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _cycleRepo = cycleRepo; _criteriaRepo = criteriaRepo; _reviewRepo = reviewRepo;
        _scoreRepo = scoreRepo; _kpiRepo = kpiRepo; _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<HrPerformanceCycleDto>> GetCyclesAsync()
        => _mapper.Map<List<HrPerformanceCycleDto>>(await _cycleRepo.GetAllAsync());

    public async Task<HrPerformanceCycleDto> CreateCycleAsync(HrPerformanceCycleDto dto)
    {
        var entity = _mapper.Map<HrPerformanceCycle>(dto); entity.Id = 0;
        await _cycleRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceCycleDto>(entity);
    }

    public async Task<HrPerformanceCycleDto> UpdateCycleAsync(HrPerformanceCycleDto dto)
    {
        var entity = await _cycleRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Cycle {dto.Id} not found.");
        entity.CycleName = dto.CycleName; entity.CycleNameAr = dto.CycleNameAr;
        entity.StartDate = dto.StartDate; entity.EndDate = dto.EndDate;
        entity.Status = dto.Status; entity.Description = dto.Description;
        _cycleRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceCycleDto>(entity);
    }

    public async Task<List<HrPerformanceCriteriaDto>> GetCriteriaAsync()
        => _mapper.Map<List<HrPerformanceCriteriaDto>>(await _criteriaRepo.Query().OrderBy(c => c.SortOrder).ToListAsync());

    public async Task<HrPerformanceCriteriaDto> CreateCriteriaAsync(HrPerformanceCriteriaDto dto)
    {
        var entity = _mapper.Map<HrPerformanceCriteria>(dto); entity.Id = 0;
        await _criteriaRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceCriteriaDto>(entity);
    }

    public async Task<HrPerformanceCriteriaDto> UpdateCriteriaAsync(HrPerformanceCriteriaDto dto)
    {
        var entity = await _criteriaRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Criteria {dto.Id} not found.");
        entity.CriteriaName = dto.CriteriaName; entity.CriteriaNameAr = dto.CriteriaNameAr;
        entity.Description = dto.Description; entity.Category = dto.Category;
        entity.Weight = dto.Weight; entity.MaxScore = dto.MaxScore;
        entity.IsActive = dto.IsActive; entity.SortOrder = dto.SortOrder;
        _criteriaRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceCriteriaDto>(entity);
    }

    public async Task DeleteCriteriaAsync(int id)
    {
        var entity = await _criteriaRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Criteria {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _criteriaRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrPerformanceReviewDto>> GetReviewsAsync(int? cycleId = null, int? employeeId = null)
    {
        var query = _reviewRepo.Query().Include(r => r.Employee).Include(r => r.PerformanceCycle).AsQueryable();
        if (cycleId.HasValue) query = query.Where(r => r.PerformanceCycleId == cycleId.Value);
        if (employeeId.HasValue) query = query.Where(r => r.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return items.Select(r => new HrPerformanceReviewDto
        {
            Id = r.Id, EmployeeId = r.EmployeeId, EmployeeName = r.Employee?.FullName,
            PerformanceCycleId = r.PerformanceCycleId, CycleName = r.PerformanceCycle?.CycleName,
            ReviewerId = r.ReviewerId, TotalScore = r.TotalScore, MaxPossibleScore = r.MaxPossibleScore,
            Percentage = r.Percentage, PerformanceRating = r.PerformanceRating,
            Recommendation = r.Recommendation, Status = r.Status, CompletedDate = r.CompletedDate
        }).ToList();
    }

    public async Task<HrPerformanceReviewDto?> GetReviewByIdAsync(int id)
    {
        var r = await _reviewRepo.Query()
            .Include(r => r.Employee).Include(r => r.PerformanceCycle)
            .Include(r => r.Scores).ThenInclude(s => s.Criteria)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (r == null) return null;
        return new HrPerformanceReviewDto
        {
            Id = r.Id, EmployeeId = r.EmployeeId, EmployeeName = r.Employee?.FullName,
            PerformanceCycleId = r.PerformanceCycleId, CycleName = r.PerformanceCycle?.CycleName,
            ReviewerId = r.ReviewerId, TotalScore = r.TotalScore, MaxPossibleScore = r.MaxPossibleScore,
            Percentage = r.Percentage, PerformanceRating = r.PerformanceRating,
            Strengths = r.Strengths, AreasForImprovement = r.AreasForImprovement, Goals = r.Goals,
            ManagerComments = r.ManagerComments, Recommendation = r.Recommendation,
            Status = r.Status, CompletedDate = r.CompletedDate, Notes = r.Notes,
            Scores = r.Scores.Select(s => new HrPerformanceScoreDto
            {
                Id = s.Id, PerformanceReviewId = s.PerformanceReviewId,
                PerformanceCriteriaId = s.PerformanceCriteriaId, CriteriaName = s.Criteria?.CriteriaName,
                Score = s.Score, MaxScore = s.MaxScore, WeightedScore = s.WeightedScore, Comments = s.Comments
            }).ToList()
        };
    }

    public async Task<HrPerformanceReviewDto> CreateReviewAsync(HrPerformanceReviewDto dto)
    {
        var entity = _mapper.Map<HrPerformanceReview>(dto); entity.Id = 0;
        entity.Status = ReviewStatus.Draft;
        await _reviewRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceReviewDto>(entity);
    }

    public async Task<HrPerformanceReviewDto> UpdateReviewAsync(HrPerformanceReviewDto dto)
    {
        var entity = await _reviewRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Review {dto.Id} not found.");
        entity.Strengths = dto.Strengths; entity.AreasForImprovement = dto.AreasForImprovement;
        entity.Goals = dto.Goals; entity.ManagerComments = dto.ManagerComments;
        entity.Recommendation = dto.Recommendation; entity.Notes = dto.Notes;
        _reviewRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPerformanceReviewDto>(entity);
    }

    public async Task CompleteReviewAsync(int id)
    {
        var entity = await _reviewRepo.Query().Include(r => r.Scores).FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException($"Review {id} not found.");
        entity.TotalScore = entity.Scores.Sum(s => s.WeightedScore);
        entity.MaxPossibleScore = entity.Scores.Sum(s => s.MaxScore);
        entity.Percentage = entity.MaxPossibleScore > 0 ? (entity.TotalScore / entity.MaxPossibleScore) * 100 : 0;
        entity.PerformanceRating = entity.Percentage switch
        {
            >= 95 => "Outstanding", >= 85 => "Excellent", >= 75 => "Good",
            >= 60 => "Satisfactory", >= 50 => "NeedsImprovement", _ => "Poor"
        };
        entity.Status = ReviewStatus.Completed;
        entity.CompletedDate = DateTime.UtcNow;
        _reviewRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrKpiDto>> GetKpisAsync(int? employeeId = null)
    {
        var query = _kpiRepo.Query().Include(k => k.Employee).AsQueryable();
        if (employeeId.HasValue) query = query.Where(k => k.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(k => k.DueDate).ToListAsync();
        return _mapper.Map<List<HrKpiDto>>(items);
    }

    public async Task<HrKpiDto> CreateKpiAsync(HrKpiDto dto)
    {
        var entity = _mapper.Map<HrKpi>(dto); entity.Id = 0; entity.Status = KpiStatus.NotStarted;
        await _kpiRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrKpiDto>(entity);
    }

    public async Task<HrKpiDto> UpdateKpiAsync(HrKpiDto dto)
    {
        var entity = await _kpiRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"KPI {dto.Id} not found.");
        entity.KpiName = dto.KpiName; entity.Description = dto.Description;
        entity.MeasurementUnit = dto.MeasurementUnit; entity.TargetValue = dto.TargetValue;
        entity.ActualValue = dto.ActualValue; entity.DueDate = dto.DueDate; entity.Notes = dto.Notes;
        entity.AchievementPercentage = dto.TargetValue > 0 ? (dto.ActualValue / dto.TargetValue) * 100 : 0;
        entity.Status = entity.AchievementPercentage >= 100 ? KpiStatus.Exceeded
            : entity.AchievementPercentage >= 90 ? KpiStatus.Achieved
            : entity.AchievementPercentage >= 70 ? KpiStatus.OnTrack : KpiStatus.Behind;
        _kpiRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrKpiDto>(entity);
    }

    public async Task DeleteKpiAsync(int id)
    {
        var entity = await _kpiRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"KPI {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _kpiRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }
}
