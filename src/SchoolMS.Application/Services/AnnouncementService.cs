using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IRepository<Announcement> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnouncementService(IRepository<Announcement> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<AnnouncementDto>> GetAllAsync(int? schoolId = null, int? branchId = null)
    {
        var query = _repository.Query().AsQueryable();
        if (schoolId.HasValue) query = query.Where(a => a.SchoolId == schoolId.Value);
        if (branchId.HasValue) query = query.Where(a => a.BranchId == branchId.Value);
        var items = await query.Include(a => a.Branch).OrderByDescending(a => a.IsPinned).ThenByDescending(a => a.CreatedAt).ToListAsync();
        return items.Select(a => new AnnouncementDto
        {
            Id = a.Id, Title = a.Title, Content = a.Content, Priority = a.Priority, Target = a.Target,
            BranchId = a.BranchId, BranchName = a.Branch?.Name, IsPinned = a.IsPinned,
            ExpiryDate = a.ExpiryDate, ViewCount = a.ViewCount, CreatedAt = a.CreatedAt,
            SchoolId = a.SchoolId
        }).ToList();
    }

    public async Task<AnnouncementDto?> GetByIdAsync(int id)
    {
        var a = await _repository.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return null;
        a.ViewCount++; _repository.Update(a); await _unitOfWork.SaveChangesAsync();
        return new AnnouncementDto
        {
            Id = a.Id, Title = a.Title, Content = a.Content, Priority = a.Priority, Target = a.Target,
            BranchId = a.BranchId, BranchName = a.Branch?.Name, IsPinned = a.IsPinned,
            ExpiryDate = a.ExpiryDate, ViewCount = a.ViewCount, CreatedAt = a.CreatedAt,
            SchoolId = a.SchoolId
        };
    }

    public async Task<AnnouncementDto> CreateAsync(AnnouncementDto dto)
    {
        var entity = new Announcement
        {
            Title = dto.Title, Content = dto.Content, Priority = dto.Priority, Target = dto.Target,
            BranchId = dto.BranchId, IsPinned = dto.IsPinned, ExpiryDate = dto.ExpiryDate, SendNotification = true,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.CreatedAt = entity.CreatedAt; return dto;
    }

    public async Task<AnnouncementDto> UpdateAsync(AnnouncementDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.Title = dto.Title; entity.Content = dto.Content; entity.Priority = dto.Priority;
        entity.Target = dto.Target; entity.BranchId = dto.BranchId; entity.IsPinned = dto.IsPinned;
        entity.ExpiryDate = dto.ExpiryDate; entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }
}

