using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrFingerprintService : IHrFingerprintService
{
    private readonly IRepository<HrFingerprintRecord> _recordRepo;
    private readonly IRepository<HrFingerprintDevice> _deviceRepo;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrFingerprintService(IRepository<HrFingerprintRecord> recordRepo,
        IRepository<HrFingerprintDevice> deviceRepo, IRepository<HrEmployee> employeeRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _recordRepo = recordRepo;
        _deviceRepo = deviceRepo;
        _employeeRepo = employeeRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HrFingerprintRecordDto> RecordScanAsync(HrFingerprintScanDto dto)
    {
        var employee = await _employeeRepo.Query()
            .FirstOrDefaultAsync(e => e.BadgeCardNumber == dto.BadgeCardNumber || e.QrCode == dto.BadgeCardNumber)
            ?? throw new KeyNotFoundException("Employee not found for given badge/QR code.");

        var now = DateTime.UtcNow;
        var record = new HrFingerprintRecord
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            EmployeeNumber = employee.EmployeeNumber,
            BadgeCardNumber = employee.BadgeCardNumber,
            RecordDate = now.Date,
            RecordTime = now.TimeOfDay,
            RecordDateTime = now,
            Type = dto.Type,
            Source = dto.Source,
            BranchId = dto.BranchId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _recordRepo.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        return new HrFingerprintRecordDto
        {
            Id = record.Id,
            EmployeeId = record.EmployeeId,
            EmployeeName = record.EmployeeName,
            EmployeeNumber = record.EmployeeNumber,
            RecordDate = record.RecordDate,
            RecordTime = record.RecordTime,
            RecordDateTime = record.RecordDateTime,
            Type = record.Type,
            Source = record.Source,
            BranchId = record.BranchId
        };
    }

    public async Task<List<HrFingerprintRecordDto>> GetRecordsAsync(DateTime? fromDate, DateTime? toDate, int? employeeId)
    {
        var query = _recordRepo.Query().AsQueryable();
        if (fromDate.HasValue) query = query.Where(r => r.RecordDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(r => r.RecordDate <= toDate.Value.Date);
        if (employeeId.HasValue) query = query.Where(r => r.EmployeeId == employeeId.Value);

        var items = await query.OrderByDescending(r => r.RecordDateTime).ToListAsync();
        return _mapper.Map<List<HrFingerprintRecordDto>>(items);
    }

    public async Task<HrFingerprintRecordDto> CreateManualEntryAsync(HrFingerprintRecordDto dto)
    {
        var employee = await _employeeRepo.GetByIdAsync(dto.EmployeeId)
            ?? throw new KeyNotFoundException($"Employee with ID {dto.EmployeeId} not found.");

        var record = new HrFingerprintRecord
        {
            EmployeeId = dto.EmployeeId,
            EmployeeName = employee.FullName,
            EmployeeNumber = employee.EmployeeNumber,
            RecordDate = dto.RecordDate,
            RecordTime = dto.RecordTime,
            RecordDateTime = dto.RecordDate.Add(dto.RecordTime),
            Type = dto.Type,
            Source = FingerprintSource.Manual,
            BranchId = dto.BranchId,
            IsManualEntry = true,
            ManualEntryReason = dto.ManualEntryReason,
            Notes = dto.Notes
        };

        await _recordRepo.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrFingerprintRecordDto>(record);
    }

    public async Task<List<HrFingerprintRecordDto>> GetTodayRecordsAsync(int employeeId)
    {
        var today = DateTime.UtcNow.Date;
        var items = await _recordRepo.Query()
            .Where(r => r.EmployeeId == employeeId && r.RecordDate == today)
            .OrderBy(r => r.RecordTime)
            .ToListAsync();
        return _mapper.Map<List<HrFingerprintRecordDto>>(items);
    }

    public async Task<List<HrFingerprintDeviceDto>> GetDevicesAsync()
        => _mapper.Map<List<HrFingerprintDeviceDto>>(await _deviceRepo.Query().Include(d => d.Branch).ToListAsync());

    public async Task<HrFingerprintDeviceDto> CreateDeviceAsync(HrFingerprintDeviceDto dto)
    {
        var entity = _mapper.Map<HrFingerprintDevice>(dto);
        entity.Id = 0;
        await _deviceRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrFingerprintDeviceDto>(entity);
    }

    public async Task<HrFingerprintDeviceDto> UpdateDeviceAsync(HrFingerprintDeviceDto dto)
    {
        var entity = await _deviceRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Device with ID {dto.Id} not found.");
        entity.DeviceName = dto.DeviceName;
        entity.DeviceModel = dto.DeviceModel;
        entity.SerialNumber = dto.SerialNumber;
        entity.IpAddress = dto.IpAddress;
        entity.Port = dto.Port;
        entity.Location = dto.Location;
        entity.BranchId = dto.BranchId;
        entity.ConnectionType = dto.ConnectionType;
        entity.IsActive = dto.IsActive;
        _deviceRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrFingerprintDeviceDto>(entity);
    }

    public async Task DeleteDeviceAsync(int id)
    {
        var entity = await _deviceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Device with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _deviceRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
