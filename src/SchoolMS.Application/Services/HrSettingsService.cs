using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrSettingsService : IHrSettingsService
{
    private readonly IRepository<School> _schoolRepo;
    private readonly IUnitOfWork _unitOfWork;

    public HrSettingsService(IRepository<School> schoolRepo, IUnitOfWork unitOfWork)
    {
        _schoolRepo = schoolRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<HrSettingsDto> GetSettingsAsync()
    {
        var schools = await _schoolRepo.GetAllAsync();
        var school = schools.FirstOrDefault();
        if (school == null) return new HrSettingsDto();
        return new HrSettingsDto
        {
            HrRequireApprovalForLeaves = school.HrRequireApprovalForLeaves,
            HrAutoCalculateOvertime = school.HrAutoCalculateOvertime,
            HrAutoDeductAbsence = school.HrAutoDeductAbsence,
            HrEnableFingerprintIntegration = school.HrEnableFingerprintIntegration,
            HrEnableSelfService = school.HrEnableSelfService,
            HrMaxOvertimeHoursPerMonth = school.HrMaxOvertimeHoursPerMonth,
            HrOvertimeRateMultiplier = school.HrOvertimeRateMultiplier,
            HrLateGracePeriodMinutes = school.HrLateGracePeriodMinutes,
            HrWorkDayStart = school.HrWorkDayStart,
            HrWorkDayEnd = school.HrWorkDayEnd,
            HrWorkingDaysPerMonth = school.HrWorkingDaysPerMonth,
            HrAbsenceDeductionPerDay = school.HrAbsenceDeductionPerDay,
            HrAbsenceDeductionType = school.HrAbsenceDeductionType,
            HrLateDeductionPerMinute = school.HrLateDeductionPerMinute,
            HrEarlyLeaveDeductionPerMinute = school.HrEarlyLeaveDeductionPerMinute,
            HrSalaryCalculationMethod = school.HrSalaryCalculationMethod
        };
    }

    public async Task UpdateSettingsAsync(HrSettingsDto dto)
    {
        var schools = await _schoolRepo.GetAllAsync();
        var school = schools.FirstOrDefault();
        if (school == null) return;

        school.HrRequireApprovalForLeaves = dto.HrRequireApprovalForLeaves;
        school.HrAutoCalculateOvertime = dto.HrAutoCalculateOvertime;
        school.HrAutoDeductAbsence = dto.HrAutoDeductAbsence;
        school.HrEnableFingerprintIntegration = dto.HrEnableFingerprintIntegration;
        school.HrEnableSelfService = dto.HrEnableSelfService;
        school.HrMaxOvertimeHoursPerMonth = dto.HrMaxOvertimeHoursPerMonth;
        school.HrOvertimeRateMultiplier = dto.HrOvertimeRateMultiplier;
        school.HrLateGracePeriodMinutes = dto.HrLateGracePeriodMinutes;
        school.HrWorkDayStart = dto.HrWorkDayStart;
        school.HrWorkDayEnd = dto.HrWorkDayEnd;
        school.HrWorkingDaysPerMonth = dto.HrWorkingDaysPerMonth;
        school.HrAbsenceDeductionPerDay = dto.HrAbsenceDeductionPerDay;
        school.HrAbsenceDeductionType = dto.HrAbsenceDeductionType;
        school.HrLateDeductionPerMinute = dto.HrLateDeductionPerMinute;
        school.HrEarlyLeaveDeductionPerMinute = dto.HrEarlyLeaveDeductionPerMinute;
        school.HrSalaryCalculationMethod = dto.HrSalaryCalculationMethod;
        _schoolRepo.Update(school);
        await _unitOfWork.SaveChangesAsync();
    }
}
