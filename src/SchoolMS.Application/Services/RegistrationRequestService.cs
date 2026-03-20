using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class RegistrationRequestService : IRegistrationRequestService
{
    private readonly IRepository<SchoolRegistrationRequest> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationRequestService(
        IRepository<SchoolRegistrationRequest> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SchoolRegistrationRequestDto>> GetAllAsync()
    {
        return await _repository.Query()
            .OrderByDescending(r => r.SubmittedAt)
            .Select(r => new SchoolRegistrationRequestDto
            {
                Id = r.Id,
                SchoolName = r.SchoolName,
                ContactName = r.ContactName,
                Phone = r.Phone,
                Email = r.Email,
                City = r.City,
                SchoolType = r.SchoolType,
                StudentCountRange = r.StudentCountRange,
                RequestedPlan = r.RequestedPlan,
                Notes = r.Notes,
                Status = r.Status,
                SubmittedAt = r.SubmittedAt,
                AssignedTo = r.AssignedTo,
                InternalNotes = r.InternalNotes,
                ContactedAt = r.ContactedAt,
                ApprovedAt = r.ApprovedAt,
                CreatedSchoolId = r.CreatedSchoolId,
                IpAddress = r.IpAddress,
                Source = r.Source
            })
            .ToListAsync();
    }

    public async Task<SchoolRegistrationRequestDto?> GetByIdAsync(int id)
    {
        var r = await _repository.GetByIdAsync(id);
        if (r == null) return null;
        return new SchoolRegistrationRequestDto
        {
            Id = r.Id,
            SchoolName = r.SchoolName,
            ContactName = r.ContactName,
            Phone = r.Phone,
            Email = r.Email,
            City = r.City,
            SchoolType = r.SchoolType,
            StudentCountRange = r.StudentCountRange,
            RequestedPlan = r.RequestedPlan,
            Notes = r.Notes,
            Status = r.Status,
            SubmittedAt = r.SubmittedAt,
            AssignedTo = r.AssignedTo,
            InternalNotes = r.InternalNotes,
            ContactedAt = r.ContactedAt,
            ApprovedAt = r.ApprovedAt,
            CreatedSchoolId = r.CreatedSchoolId,
            IpAddress = r.IpAddress,
            Source = r.Source
        };
    }

    public async Task<SchoolRegistrationRequestDto> CreateAsync(CreateRegistrationRequestDto dto, string? ipAddress)
    {
        var entity = new SchoolRegistrationRequest
        {
            SchoolName = dto.SchoolName,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            City = dto.City,
            SchoolType = dto.SchoolType,
            StudentCountRange = dto.StudentCountRange,
            RequestedPlan = dto.RequestedPlan,
            Notes = dto.Notes,
            Status = RequestStatus.New,
            SubmittedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            Source = "Website"
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new SchoolRegistrationRequestDto
        {
            Id = entity.Id,
            SchoolName = entity.SchoolName,
            ContactName = entity.ContactName,
            Phone = entity.Phone,
            Email = entity.Email,
            Status = entity.Status,
            SubmittedAt = entity.SubmittedAt
        };
    }

    public async Task UpdateStatusAsync(UpdateRequestStatusDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Registration request not found.");

        entity.Status = dto.Status;
        if (!string.IsNullOrEmpty(dto.InternalNotes))
            entity.InternalNotes = dto.InternalNotes;

        if (dto.Status == RequestStatus.Contacted && entity.ContactedAt == null)
            entity.ContactedAt = DateTime.UtcNow;

        if (dto.Status == RequestStatus.Approved && entity.ApprovedAt == null)
            entity.ApprovedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<RegistrationRequestStatsDto> GetStatsAsync()
    {
        var all = await _repository.GetAllAsync();
        var requests = all.ToList();
        var total = requests.Count;
        var approved = requests.Count(r => r.Status == RequestStatus.Approved);
        return new RegistrationRequestStatsDto
        {
            TotalRequests = total,
            NewRequests = requests.Count(r => r.Status == RequestStatus.New),
            ApprovedRequests = approved,
            RejectedRequests = requests.Count(r => r.Status == RequestStatus.Rejected),
            ConversionRate = total > 0 ? Math.Round((double)approved / total * 100, 1) : 0
        };
    }
}
