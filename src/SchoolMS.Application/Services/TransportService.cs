using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class TransportService : ITransportService
{
    private readonly IRepository<TransportRoute> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TransportService(IRepository<TransportRoute> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<TransportRouteDto>> GetAllRoutesAsync()
    {
        var items = await _repository.Query().Include(r => r.Branch).Include(r => r.Stops).ToListAsync();
        return items.Select(r => new TransportRouteDto
        {
            Id = r.Id, RouteName = r.RouteName, DriverName = r.DriverName, DriverPhone = r.DriverPhone,
            BusNumber = r.BusNumber, Capacity = r.Capacity, CurrentPassengers = r.CurrentPassengers,
            MonthlyFee = r.MonthlyFee, BranchId = r.BranchId, BranchName = r.Branch?.Name,
            IsActive = r.IsActive, StopCount = r.Stops.Count(s => !s.IsDeleted),
            SchoolId = r.SchoolId
        }).ToList();
    }

    public async Task<TransportRouteDto?> GetRouteByIdAsync(int id)
    {
        var r = await _repository.Query().Include(x => x.Branch).Include(x => x.Stops).FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return null;
        return new TransportRouteDto
        {
            Id = r.Id, RouteName = r.RouteName, DriverName = r.DriverName, DriverPhone = r.DriverPhone,
            BusNumber = r.BusNumber, Capacity = r.Capacity, CurrentPassengers = r.CurrentPassengers,
            MonthlyFee = r.MonthlyFee, BranchId = r.BranchId, BranchName = r.Branch?.Name,
            IsActive = r.IsActive, StopCount = r.Stops.Count(s => !s.IsDeleted),
            SchoolId = r.SchoolId
        };
    }

    public async Task<TransportRouteDto> CreateRouteAsync(TransportRouteDto dto)
    {
        var entity = new TransportRoute
        {
            RouteName = dto.RouteName, DriverName = dto.DriverName, DriverPhone = dto.DriverPhone,
            BusNumber = dto.BusNumber, Capacity = dto.Capacity, MonthlyFee = dto.MonthlyFee,
            BranchId = dto.BranchId, IsActive = true,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task<TransportRouteDto> UpdateRouteAsync(TransportRouteDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.RouteName = dto.RouteName; entity.DriverName = dto.DriverName; entity.DriverPhone = dto.DriverPhone;
        entity.BusNumber = dto.BusNumber; entity.Capacity = dto.Capacity; entity.MonthlyFee = dto.MonthlyFee;
        entity.BranchId = dto.BranchId; entity.IsActive = dto.IsActive;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteRouteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }
}

