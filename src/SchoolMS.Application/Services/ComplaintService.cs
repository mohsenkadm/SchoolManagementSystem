using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IRepository<Complaint> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ComplaintService(IRepository<Complaint> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<ComplaintDto>> GetAllAsync()
    {
        var items = await _repository.Query().OrderByDescending(c => c.CreatedAt).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<ComplaintDto?> GetByIdAsync(int id)
    {
        var c = await _repository.GetByIdAsync(id);
        return c == null ? null : MapToDto(c);
    }

    public async Task<ComplaintDto> CreateAsync(ComplaintDto dto)
    {
        var entity = new Complaint
        {
            TicketNumber = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PersonName = dto.PersonName, Subject = dto.Subject, Description = dto.Description,
            Category = dto.Category, Priority = dto.Priority, Status = ComplaintStatus.Open,
            IsAnonymous = dto.IsAnonymous, SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.TicketNumber = entity.TicketNumber; dto.CreatedAt = entity.CreatedAt; return dto;
    }

    public async Task<ComplaintDto> UpdateAsync(ComplaintDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.Subject = dto.Subject; entity.Description = dto.Description; entity.Category = dto.Category;
        entity.Priority = dto.Priority; entity.Status = dto.Status; entity.AssignedTo = dto.AssignedTo;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    public async Task ResolveAsync(int id, string resolution)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.Status = ComplaintStatus.Resolved; entity.Resolution = resolution; entity.ResolvedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    private static ComplaintDto MapToDto(Complaint c) => new()
    {
        Id = c.Id, TicketNumber = c.TicketNumber, PersonName = c.PersonName, Subject = c.Subject,
        Description = c.Description, Category = c.Category, Priority = c.Priority, Status = c.Status,
        AssignedTo = c.AssignedTo, Resolution = c.Resolution, ResolvedAt = c.ResolvedAt,
        IsAnonymous = c.IsAnonymous, CreatedAt = c.CreatedAt,
        SchoolId = c.SchoolId
    };
}

