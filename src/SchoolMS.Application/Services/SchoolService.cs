using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class SchoolService : ISchoolService
{
    private readonly IRepository<School> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SchoolService(IRepository<School> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }


    public async Task<List<SchoolDto>> GetBySchoolIdAsync(int schoolId)
    {
        var query = _repository.Query()
            .Include(s => s.Branches)
            .Include(s => s.SchoolSubscriptions)
                .ThenInclude(ss => ss.SystemSubscriptionPlan)
            .Where(e => e.Id == schoolId);
        var items = await query.ToListAsync();
        return items.Select(e => new SchoolDto
        {
            Id = e.Id,
            Name = e.Name,
            Logo = e.Logo,
            Address = e.Address,
            Slug = e.Slug,
            IsActive = e.IsActive,
            ExpiryDate = e.ExpiryDate,
            OnlinePlatformEnabled = e.OnlinePlatformEnabled,
            BranchCount = e.Branches.Count,
            StudentCount = 0,
            TeacherCount = 0,
            CurrentPlan = e.SchoolSubscriptions
                .Where(s => s.IsActive)
                .Select(s => s.SystemSubscriptionPlan?.PlanName)
                .FirstOrDefault(),
            CreatedAt = e.CreatedAt
        }).ToList();
    }
}