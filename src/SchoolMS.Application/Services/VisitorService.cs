using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IRepository<Visitor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public VisitorService(IRepository<Visitor> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<VisitorDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(v => v.Branch).OrderByDescending(v => v.CheckInTime).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<VisitorDto?> GetByIdAsync(int id)
    {
        var v = await _repository.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        return v == null ? null : MapToDto(v);
    }

    public async Task<VisitorDto> CheckInAsync(VisitorDto dto)
    {
        var entity = new Visitor
        {
            VisitorName = dto.VisitorName, Phone = dto.Phone, NationalId = dto.NationalId,
            Purpose = dto.Purpose, VisitingPerson = dto.VisitingPerson, VisitingDepartment = dto.VisitingDepartment,
            CheckInTime = DateTime.UtcNow, BranchId = dto.BranchId, Status = VisitorStatus.CheckedIn, Notes = dto.Notes,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.CheckInTime = entity.CheckInTime; dto.Status = entity.Status; return dto;
    }

    public async Task CheckOutAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.CheckOutTime = DateTime.UtcNow; entity.Status = VisitorStatus.CheckedOut;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    private static VisitorDto MapToDto(Visitor v) => new()
    {
        Id = v.Id, VisitorName = v.VisitorName, Phone = v.Phone, NationalId = v.NationalId,
        Purpose = v.Purpose, VisitingPerson = v.VisitingPerson, VisitingDepartment = v.VisitingDepartment,
        CheckInTime = v.CheckInTime, CheckOutTime = v.CheckOutTime, BranchId = v.BranchId,
        BranchName = v.Branch?.Name, Status = v.Status, Notes = v.Notes,
        SchoolId = v.SchoolId
    };
}

