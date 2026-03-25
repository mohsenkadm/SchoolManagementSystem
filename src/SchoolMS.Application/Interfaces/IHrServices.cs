using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IHrEmployeeService
{
    Task<List<HrEmployeeListDto>> GetAllAsync();
    Task<List<HrEmployeeListDto>> GetBySchoolIdAsync(int schoolId);
    Task<HrEmployeeDto?> GetByIdAsync(int id);
    Task<HrEmployeeDto> CreateAsync(HrEmployeeDto dto);
    Task<HrEmployeeDto> UpdateAsync(HrEmployeeDto dto);
    Task DeleteAsync(int id);
    Task<List<HrEmployeeListDto>> GetByDepartmentAsync(int departmentId);
    Task<List<HrEmployeeListDto>> GetByBranchAsync(int branchId);
    Task<string> GenerateEmployeeNumberAsync();
}

public interface IHrDepartmentService
{
    Task<List<HrDepartmentDto>> GetAllAsync();
    Task<List<HrDepartmentDto>> GetBySchoolIdAsync(int schoolId);
    Task<HrDepartmentDto?> GetByIdAsync(int id);
    Task<HrDepartmentDto> CreateAsync(HrDepartmentDto dto);
    Task<HrDepartmentDto> UpdateAsync(HrDepartmentDto dto);
    Task DeleteAsync(int id);
}

public interface IHrJobTitleService
{
    Task<List<HrJobTitleDto>> GetAllAsync();
    Task<List<HrJobTitleDto>> GetBySchoolIdAsync(int schoolId);
    Task<HrJobTitleDto?> GetByIdAsync(int id);
    Task<HrJobTitleDto> CreateAsync(HrJobTitleDto dto);
    Task<HrJobTitleDto> UpdateAsync(HrJobTitleDto dto);
    Task DeleteAsync(int id);
}

public interface IHrJobGradeService
{
    Task<List<HrJobGradeDto>> GetAllAsync();
    Task<List<HrJobGradeDto>> GetBySchoolIdAsync(int schoolId);
    Task<HrJobGradeDto?> GetByIdAsync(int id);
    Task<HrJobGradeDto> CreateAsync(HrJobGradeDto dto);
    Task<HrJobGradeDto> UpdateAsync(HrJobGradeDto dto);
    Task DeleteAsync(int id);
    Task<List<HrJobGradeStepDto>> GetStepsAsync(int gradeId);
    Task<HrJobGradeStepDto> CreateStepAsync(HrJobGradeStepDto dto);
    Task DeleteStepAsync(int stepId);
}

public interface IHrContractService
{
    Task<List<HrEmployeeContractDto>> GetAllAsync();
    Task<List<HrEmployeeContractDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<HrEmployeeContractDto>> GetByEmployeeAsync(int employeeId);
    Task<HrEmployeeContractDto?> GetByIdAsync(int id);
    Task<HrEmployeeContractDto> CreateAsync(HrEmployeeContractDto dto);
    Task<HrEmployeeContractDto> UpdateAsync(HrEmployeeContractDto dto);
    Task DeleteAsync(int id);
}

public interface IHrWorkShiftService
{
    Task<List<HrWorkShiftDto>> GetAllAsync();
    Task<List<HrWorkShiftDto>> GetBySchoolIdAsync(int schoolId);
    Task<HrWorkShiftDto?> GetByIdAsync(int id);
    Task<HrWorkShiftDto> CreateAsync(HrWorkShiftDto dto);
    Task<HrWorkShiftDto> UpdateAsync(HrWorkShiftDto dto);
    Task DeleteAsync(int id);
}

public interface IHrFingerprintService
{
    Task<HrFingerprintRecordDto> RecordScanAsync(HrFingerprintScanDto dto);
    Task<List<HrFingerprintRecordDto>> GetRecordsAsync(DateTime? fromDate, DateTime? toDate, int? employeeId);
    Task<HrFingerprintRecordDto> CreateManualEntryAsync(HrFingerprintRecordDto dto);
    Task<List<HrFingerprintRecordDto>> GetTodayRecordsAsync(int employeeId);

    // Devices
    Task<List<HrFingerprintDeviceDto>> GetDevicesAsync();
    Task<HrFingerprintDeviceDto> CreateDeviceAsync(HrFingerprintDeviceDto dto);
    Task<HrFingerprintDeviceDto> UpdateDeviceAsync(HrFingerprintDeviceDto dto);
    Task DeleteDeviceAsync(int id);
}

public interface IHrAttendanceService
{
    Task<List<HrDailyAttendanceDto>> GetDailyAttendanceAsync(DateTime date, int? departmentId = null, int? branchId = null);
    Task<List<HrDailyAttendanceDto>> GetMonthlyAttendanceAsync(int employeeId, int month, int year);
    Task ProcessDailyAttendanceAsync(DateTime date);
}

public interface IHrOvertimeService
{
    Task<List<HrOvertimeRequestDto>> GetAllAsync(OvertimeStatus? status = null);
    Task<List<HrOvertimeRequestDto>> GetBySchoolIdAsync(int schoolId, OvertimeStatus? status = null);
    Task<HrOvertimeRequestDto> CreateAsync(HrOvertimeRequestDto dto);
    Task ApproveAsync(int id, string approvedBy);
    Task RejectAsync(int id, string rejectedBy, string reason);
}

public interface IHrSalaryService
{
    // Salary Setup
    Task<HrSalaryDetailDto?> GetCurrentSalaryAsync(int employeeId);
    Task<List<HrSalaryDetailDto>> GetSalaryHistoryAsync(int employeeId);
    Task<HrSalaryDetailDto> CreateSalarySetupAsync(HrSalaryDetailDto dto);
    Task<HrSalaryDetailDto> UpdateSalarySetupAsync(HrSalaryDetailDto dto);

    // Allowance/Deduction Types
    Task<List<HrAllowanceTypeDto>> GetAllowanceTypesAsync();
    Task<HrAllowanceTypeDto> CreateAllowanceTypeAsync(HrAllowanceTypeDto dto);
    Task<HrAllowanceTypeDto> UpdateAllowanceTypeAsync(HrAllowanceTypeDto dto);
    Task DeleteAllowanceTypeAsync(int id);
    Task<List<HrDeductionTypeDto>> GetDeductionTypesAsync();
    Task<HrDeductionTypeDto> CreateDeductionTypeAsync(HrDeductionTypeDto dto);
    Task<HrDeductionTypeDto> UpdateDeductionTypeAsync(HrDeductionTypeDto dto);
    Task DeleteDeductionTypeAsync(int id);

    // Payroll
    Task<HrMonthlyPayrollDto> GeneratePayrollAsync(int month, int year, int branchId);
    Task<HrMonthlyPayrollDto?> GetPayrollAsync(int month, int year, int branchId);
    Task<List<HrMonthlyPayrollDto>> GetPayrollListAsync(int? year = null);
    Task ApprovePayrollAsync(int payrollId, string approvedBy);
    Task MarkPayrollPaidAsync(int payrollId);

    // Advances
    Task<List<HrSalaryAdvanceDto>> GetAdvancesAsync(AdvanceStatus? status = null);
    Task<List<HrSalaryAdvanceDto>> GetAdvancesBySchoolIdAsync(int schoolId, AdvanceStatus? status = null);
    Task<HrSalaryAdvanceDto> CreateAdvanceAsync(HrSalaryAdvanceDto dto);
    Task ApproveAdvanceAsync(int id, string approvedBy, decimal approvedAmount, int deductionMonths);
    Task RejectAdvanceAsync(int id, string rejectedBy, string reason);

    // Loans
    Task<List<HrEmployeeLoanDto>> GetLoansAsync(int? employeeId = null);
    Task<List<HrEmployeeLoanDto>> GetLoansBySchoolIdAsync(int schoolId, int? employeeId = null);
    Task<HrEmployeeLoanDto> CreateLoanAsync(HrEmployeeLoanDto dto);

    // Bonuses
    Task<List<HrBonusDto>> GetBonusesAsync(int? month = null, int? year = null);
    Task<List<HrBonusDto>> GetBonusesBySchoolIdAsync(int schoolId, int? month = null, int? year = null);
    Task<HrBonusDto> CreateBonusAsync(HrBonusDto dto);
    Task ApproveBonusAsync(int id, string approvedBy);

    // Penalties
    Task<List<HrPenaltyDto>> GetPenaltiesAsync(int? month = null, int? year = null);
    Task<List<HrPenaltyDto>> GetPenaltiesBySchoolIdAsync(int schoolId, int? month = null, int? year = null);
    Task<HrPenaltyDto> CreatePenaltyAsync(HrPenaltyDto dto);
    Task ApprovePenaltyAsync(int id, string approvedBy);
}

public interface IHrPromotionService
{
    Task<List<HrPromotionDto>> GetAllAsync(HrPromotionStatus? status = null);
    Task<List<HrPromotionDto>> GetBySchoolIdAsync(int schoolId, HrPromotionStatus? status = null);
    Task<HrPromotionDto?> GetByIdAsync(int id);
    Task<HrPromotionDto> CreateAsync(HrPromotionDto dto);
    Task ApproveAsync(int id, string approvedBy);
    Task RejectAsync(int id, string rejectedBy, string reason);
    Task<List<HrCareerHistoryDto>> GetCareerHistoryAsync(int employeeId);
}

public interface IHrLeaveService
{
    Task<List<HrLeaveRequestDto>> GetAllRequestsAsync(HrLeaveStatus? status = null);
    Task<List<HrLeaveRequestDto>> GetRequestsBySchoolIdAsync(int schoolId, HrLeaveStatus? status = null);
    Task<HrLeaveRequestDto> CreateRequestAsync(HrLeaveRequestDto dto);
    Task ApproveByManagerAsync(int id, string approvedBy);
    Task ApproveByHrAsync(int id, string approvedBy);
    Task RejectAsync(int id, string rejectedBy, string reason);
    Task CancelAsync(int id);

    // Leave Types
    Task<List<HrLeaveTypeDto>> GetLeaveTypesAsync();
    Task<HrLeaveTypeDto> CreateLeaveTypeAsync(HrLeaveTypeDto dto);
    Task<HrLeaveTypeDto> UpdateLeaveTypeAsync(HrLeaveTypeDto dto);
    Task DeleteLeaveTypeAsync(int id);

    // Leave Balances
    Task<List<HrLeaveBalanceDto>> GetBalancesAsync(int employeeId);
    Task<List<HrLeaveBalanceDto>> GetAllBalancesAsync(int year);
    Task InitializeBalancesAsync(int employeeId, int year);

    // Holidays
    Task<List<HrHolidayDto>> GetHolidaysAsync(int? year = null);
    Task<List<HrHolidayDto>> GetHolidaysBySchoolIdAsync(int schoolId, int? year = null);
    Task<HrHolidayDto> CreateHolidayAsync(HrHolidayDto dto);
    Task<HrHolidayDto> UpdateHolidayAsync(HrHolidayDto dto);
    Task DeleteHolidayAsync(int id);
}

public interface IHrPerformanceService
{
    // Cycles
    Task<List<HrPerformanceCycleDto>> GetCyclesAsync();
    Task<List<HrPerformanceCycleDto>> GetCyclesBySchoolIdAsync(int schoolId);
    Task<HrPerformanceCycleDto> CreateCycleAsync(HrPerformanceCycleDto dto);
    Task<HrPerformanceCycleDto> UpdateCycleAsync(HrPerformanceCycleDto dto);

    // Criteria
    Task<List<HrPerformanceCriteriaDto>> GetCriteriaAsync();
    Task<HrPerformanceCriteriaDto> CreateCriteriaAsync(HrPerformanceCriteriaDto dto);
    Task<HrPerformanceCriteriaDto> UpdateCriteriaAsync(HrPerformanceCriteriaDto dto);
    Task DeleteCriteriaAsync(int id);

    // Reviews
    Task<List<HrPerformanceReviewDto>> GetReviewsAsync(int? cycleId = null, int? employeeId = null);
    Task<List<HrPerformanceReviewDto>> GetReviewsBySchoolIdAsync(int schoolId, int? cycleId = null, int? employeeId = null);
    Task<HrPerformanceReviewDto?> GetReviewByIdAsync(int id);
    Task<HrPerformanceReviewDto> CreateReviewAsync(HrPerformanceReviewDto dto);
    Task<HrPerformanceReviewDto> UpdateReviewAsync(HrPerformanceReviewDto dto);
    Task CompleteReviewAsync(int id);

    // KPIs
    Task<List<HrKpiDto>> GetKpisAsync(int? employeeId = null);
    Task<List<HrKpiDto>> GetKpisBySchoolIdAsync(int schoolId, int? employeeId = null);
    Task<HrKpiDto> CreateKpiAsync(HrKpiDto dto);
    Task<HrKpiDto> UpdateKpiAsync(HrKpiDto dto);
    Task DeleteKpiAsync(int id);
}

public interface IHrTrainingService
{
    Task<List<HrTrainingProgramDto>> GetProgramsAsync();
    Task<List<HrTrainingProgramDto>> GetProgramsBySchoolIdAsync(int schoolId);
    Task<HrTrainingProgramDto?> GetProgramByIdAsync(int id);
    Task<HrTrainingProgramDto> CreateProgramAsync(HrTrainingProgramDto dto);
    Task<HrTrainingProgramDto> UpdateProgramAsync(HrTrainingProgramDto dto);
    Task DeleteProgramAsync(int id);

    Task<List<HrTrainingRecordDto>> GetRecordsAsync(int? programId = null, int? employeeId = null);
    Task<List<HrTrainingRecordDto>> GetRecordsBySchoolIdAsync(int schoolId, int? programId = null, int? employeeId = null);
    Task<HrTrainingRecordDto> EnrollEmployeeAsync(HrTrainingRecordDto dto);
    Task<HrTrainingRecordDto> UpdateRecordAsync(HrTrainingRecordDto dto);

    Task<List<HrTrainingRequestDto>> GetRequestsAsync(TrainingRequestStatus? status = null);
    Task<HrTrainingRequestDto> CreateRequestAsync(HrTrainingRequestDto dto);
    Task ApproveRequestAsync(int id, string approvedBy);
    Task RejectRequestAsync(int id, string rejectedBy);

    Task<List<HrProfessionalCertificateDto>> GetCertificatesAsync(int employeeId);
    Task<HrProfessionalCertificateDto> CreateCertificateAsync(HrProfessionalCertificateDto dto);
    Task DeleteCertificateAsync(int id);
}

public interface IHrDisciplinaryService
{
    Task<List<HrDisciplinaryActionDto>> GetAllAsync(int? employeeId = null);
    Task<List<HrDisciplinaryActionDto>> GetBySchoolIdAsync(int schoolId, int? employeeId = null);
    Task<HrDisciplinaryActionDto?> GetByIdAsync(int id);
    Task<HrDisciplinaryActionDto> CreateAsync(HrDisciplinaryActionDto dto);
    Task<HrDisciplinaryActionDto> UpdateAsync(HrDisciplinaryActionDto dto);

    Task<List<HrViolationTypeDto>> GetViolationTypesAsync();
    Task<List<HrViolationTypeDto>> GetViolationTypesBySchoolIdAsync(int schoolId);
    Task<HrViolationTypeDto> CreateViolationTypeAsync(HrViolationTypeDto dto);
    Task<HrViolationTypeDto> UpdateViolationTypeAsync(HrViolationTypeDto dto);
    Task DeleteViolationTypeAsync(int id);
}

public interface IHrEndOfServiceService
{
    Task<List<HrEndOfServiceDto>> GetAllAsync();
    Task<HrEndOfServiceDto?> GetByIdAsync(int id);
    Task<HrEndOfServiceDto> CreateAsync(HrEndOfServiceDto dto);
    Task<HrEndOfServiceDto> CalculateSettlementAsync(int employeeId, EndOfServiceType type, DateTime effectiveDate);
    Task ApproveAsync(int id, string approvedBy);
    Task MarkSettledAsync(int id, string paymentMethod, string paymentReference);
}

public interface IHrDashboardService
{
    Task<HrDashboardDto> GetDashboardAsync(int? branchId = null);
}

public interface IHrSettingsService
{
    Task<HrSettingsDto> GetSettingsAsync();
    Task UpdateSettingsAsync(HrSettingsDto dto);
}
