using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class SchoolEventService : ISchoolEventService
{
    private readonly IRepository<SchoolEvent> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SchoolEventService(IRepository<SchoolEvent> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<SchoolEventDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(e => e.Branch).ToListAsync();
        return items.Select(e => new SchoolEventDto
        {
            Id = e.Id, Title = e.Title, Description = e.Description, StartDate = e.StartDate, EndDate = e.EndDate,
            Location = e.Location, EventCategory = e.EventCategory, Color = e.Color, IsAllDay = e.IsAllDay,
            BranchId = e.BranchId, BranchName = e.Branch?.Name, OrganizerName = e.OrganizerName,
            SchoolId = e.SchoolId, SchoolName = e.School?.Name
        }).ToList();
    }

    public async Task<List<SchoolEventDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _repository.Query().Where(e => e.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(e => e.BranchId == branchId.Value);
        var items = await query.Include(e => e.Branch).ToListAsync();
        return items.Select(e => new SchoolEventDto
        {
            Id = e.Id, Title = e.Title, Description = e.Description, StartDate = e.StartDate, EndDate = e.EndDate,
            Location = e.Location, EventCategory = e.EventCategory, Color = e.Color, IsAllDay = e.IsAllDay,
            BranchId = e.BranchId, BranchName = e.Branch?.Name, OrganizerName = e.OrganizerName,
            SchoolId = e.SchoolId, SchoolName = e.School?.Name
        }).ToList();
    }

    public async Task<SchoolEventDto?> GetByIdAsync(int id)
    {
        var e = await _repository.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return null;
        return new SchoolEventDto
        {
            Id = e.Id, Title = e.Title, Description = e.Description, StartDate = e.StartDate, EndDate = e.EndDate,
            Location = e.Location, EventCategory = e.EventCategory, Color = e.Color, IsAllDay = e.IsAllDay,
            BranchId = e.BranchId, BranchName = e.Branch?.Name, OrganizerName = e.OrganizerName,
            SchoolId = e.SchoolId
        };
    }

    public async Task<SchoolEventDto> CreateAsync(SchoolEventDto dto)
    {
        var entity = new SchoolEvent
        {
            Title = dto.Title, Description = dto.Description, StartDate = dto.StartDate, EndDate = dto.EndDate,
            Location = dto.Location, EventCategory = dto.EventCategory, Color = dto.Color ?? "#4361ee",
            IsAllDay = dto.IsAllDay, BranchId = dto.BranchId, OrganizerName = dto.OrganizerName,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task<SchoolEventDto> UpdateAsync(SchoolEventDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.Title = dto.Title; entity.Description = dto.Description; entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate; entity.Location = dto.Location; entity.EventCategory = dto.EventCategory;
        entity.Color = dto.Color; entity.IsAllDay = dto.IsAllDay; entity.BranchId = dto.BranchId;
        entity.OrganizerName = dto.OrganizerName; entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }
}

