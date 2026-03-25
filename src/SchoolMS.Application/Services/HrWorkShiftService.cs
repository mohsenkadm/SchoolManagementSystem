using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrWorkShiftService : IHrWorkShiftService
{
    private readonly IRepository<HrWorkShift> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrWorkShiftService(IRepository<HrWorkShift> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrWorkShiftDto>> GetAllAsync()
        => _mapper.Map<List<HrWorkShiftDto>>(await _repository.GetAllAsync());

    public async Task<List<HrWorkShiftDto>> GetBySchoolIdAsync(int schoolId)
        => _mapper.Map<List<HrWorkShiftDto>>(await _repository.Query().Where(w => w.SchoolId == schoolId).ToListAsync());

    public async Task<HrWorkShiftDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<HrWorkShiftDto>(entity);
    }

    public async Task<HrWorkShiftDto> CreateAsync(HrWorkShiftDto dto)
    {
        var entity = _mapper.Map<HrWorkShift>(dto);
        entity.Id = 0;
        entity.TotalWorkHours = (decimal)(entity.EndTime - entity.StartTime).TotalHours - (entity.BreakDurationMinutes / 60m);
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrWorkShiftDto>(entity);
    }

    public async Task<HrWorkShiftDto> UpdateAsync(HrWorkShiftDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"WorkShift with ID {dto.Id} not found.");
        entity.ShiftName = dto.ShiftName;
        entity.ShiftNameAr = dto.ShiftNameAr;
        entity.ShiftCode = dto.ShiftCode;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.BreakStartTime = dto.BreakStartTime;
        entity.BreakEndTime = dto.BreakEndTime;
        entity.BreakDurationMinutes = dto.BreakDurationMinutes;
        entity.TotalWorkHours = (decimal)(dto.EndTime - dto.StartTime).TotalHours - (dto.BreakDurationMinutes / 60m);
        entity.GracePeriodMinutes = dto.GracePeriodMinutes;
        entity.EarlyLeaveGraceMinutes = dto.EarlyLeaveGraceMinutes;
        entity.IsFlexible = dto.IsFlexible;
        entity.WorkingDays = dto.WorkingDays;
        entity.IsNightShift = dto.IsNightShift;
        entity.IsDefault = dto.IsDefault;
        entity.Color = dto.Color;
        entity.IsActive = dto.IsActive;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrWorkShiftDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"WorkShift with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
