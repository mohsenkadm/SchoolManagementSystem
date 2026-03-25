using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

// ============ Department ============
public class HrDepartmentDto
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? DepartmentNameAr { get; set; }
    public string? DepartmentCode { get; set; }
    public string? Description { get; set; }
    public int? ParentDepartmentId { get; set; }
    public string? ParentDepartmentName { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public string? ManagerName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool IsActive { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int EmployeeCount { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Job Title ============
public class HrJobTitleDto
{
    public int Id { get; set; }
    public string TitleName { get; set; } = string.Empty;
    public string? TitleNameAr { get; set; }
    public string? TitleCode { get; set; }
    public string? Description { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool IsActive { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Job Grade ============
public class HrJobGradeDto
{
    public int Id { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public string? GradeNameAr { get; set; }
    public int GradeLevel { get; set; }
    public string? Description { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public decimal DefaultAllowancePercentage { get; set; }
    public int? MinYearsExperience { get; set; }
    public bool IsActive { get; set; }
    public string? SchoolName { get; set; }
}

public class HrJobGradeStepDto
{
    public int Id { get; set; }
    public int JobGradeId { get; set; }
    public string? GradeName { get; set; }
    public int StepNumber { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal AnnualIncrement { get; set; }
    public int YearsInStep { get; set; }
}

// ============ Employee ============
public class HrEmployeeDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string? ThirdName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Nationality { get; set; }
    public string? Religion { get; set; }
    public string? MaritalStatus { get; set; }
    public int? NumberOfDependents { get; set; }
    public string? BloodType { get; set; }
    public string? ProfileImage { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Email { get; set; }
    public string? PersonalEmail { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int JobTitleId { get; set; }
    public string? JobTitleName { get; set; }
    public int? JobGradeId { get; set; }
    public string? JobGradeName { get; set; }
    public int? JobGradeStepId { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int? WorkShiftId { get; set; }
    public string? WorkShiftName { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public EmployeeCategory Category { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? ProbationEndDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public HrEmployeeStatus Status { get; set; }
    public int? DirectManagerId { get; set; }
    public string? DirectManagerName { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? FingerprintId { get; set; }
    public string? QrCode { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? IBAN { get; set; }
    public string? Currency { get; set; }
    public string? TaxNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? HighestQualification { get; set; }
    public string? University { get; set; }
    public string? Major { get; set; }
    public int? GraduationYear { get; set; }
    public int YearsOfExperience { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

public class HrEmployeeListDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string? DepartmentName { get; set; }
    public string? JobTitleName { get; set; }
    public string? BranchName { get; set; }
    public string? Phone { get; set; }
    public HrEmployeeStatus Status { get; set; }
    public DateTime HireDate { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public EmployeeCategory Category { get; set; }
}

// ============ Contract ============
public class HrEmployeeContractDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? ContractNumber { get; set; }
    public ContractType ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal AgreedSalary { get; set; }
    public string? Terms { get; set; }
    public string? AttachmentPath { get; set; }
    public ContractStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? SignedBy { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Document ============
public class HrEmployeeDocumentDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public string? Notes { get; set; }
}

// ============ Work Shift ============
public class HrWorkShiftDto
{
    public int Id { get; set; }
    public string ShiftName { get; set; } = string.Empty;
    public string? ShiftNameAr { get; set; }
    public string? ShiftCode { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public int BreakDurationMinutes { get; set; }
    public decimal TotalWorkHours { get; set; }
    public int GracePeriodMinutes { get; set; }
    public int EarlyLeaveGraceMinutes { get; set; }
    public bool IsFlexible { get; set; }
    public string? WorkingDays { get; set; }
    public bool IsNightShift { get; set; }
    public bool IsDefault { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Fingerprint ============
public class HrFingerprintDeviceDto
{
    public int Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string? DeviceModel { get; set; }
    public string? SerialNumber { get; set; }
    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public string? Location { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public string? ConnectionType { get; set; }
    public bool IsActive { get; set; }
}

public class HrFingerprintRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public DateTime RecordDate { get; set; }
    public TimeSpan RecordTime { get; set; }
    public DateTime RecordDateTime { get; set; }
    public FingerprintType Type { get; set; }
    public FingerprintSource Source { get; set; }
    public int? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public int BranchId { get; set; }
    public bool IsManualEntry { get; set; }
    public string? ManualEntryReason { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

public class HrFingerprintScanDto
{
    public string BadgeCardNumber { get; set; } = string.Empty;
    public FingerprintType Type { get; set; }
    public FingerprintSource Source { get; set; }
    public int BranchId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

// ============ Daily Attendance ============
public class HrDailyAttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string? ShiftName { get; set; }
    public TimeSpan? FirstCheckIn { get; set; }
    public TimeSpan? LastCheckOut { get; set; }
    public decimal? TotalWorkHours { get; set; }
    public decimal? OvertimeHours { get; set; }
    public int? LateMinutes { get; set; }
    public int? EarlyLeaveMinutes { get; set; }
    public DailyAttendanceStatus Status { get; set; }
    public decimal? TotalDeductionAmount { get; set; }
    public decimal? OvertimeAmount { get; set; }
    public bool IsProcessed { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Overtime ============
public class HrOvertimeRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime OvertimeDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Hours { get; set; }
    public decimal RateMultiplier { get; set; }
    public decimal CalculatedAmount { get; set; }
    public string? Reason { get; set; }
    public OvertimeStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsFromAttendance { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Salary ============
public class HrSalaryDetailDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public decimal BaseSalary { get; set; }
    public string? Currency { get; set; }
    public SalaryType SalaryType { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Notes { get; set; }
    public List<HrSalaryAllowanceDto> Allowances { get; set; } = new();
    public List<HrSalaryDeductionDto> Deductions { get; set; } = new();
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class HrAllowanceTypeDto
{
    public int Id { get; set; }
    public string AllowanceName { get; set; } = string.Empty;
    public string? AllowanceNameAr { get; set; }
    public string? AllowanceCode { get; set; }
    public AllowanceCalculation CalculationType { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class HrSalaryAllowanceDto
{
    public int Id { get; set; }
    public int SalaryDetailId { get; set; }
    public int AllowanceTypeId { get; set; }
    public string? AllowanceName { get; set; }
    public AllowanceCalculation CalculationType { get; set; }
    public decimal Value { get; set; }
    public decimal CalculatedAmount { get; set; }
    public bool IsActive { get; set; }
}

public class HrDeductionTypeDto
{
    public int Id { get; set; }
    public string DeductionName { get; set; } = string.Empty;
    public string? DeductionNameAr { get; set; }
    public string? DeductionCode { get; set; }
    public DeductionCalculation CalculationType { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class HrSalaryDeductionDto
{
    public int Id { get; set; }
    public int SalaryDetailId { get; set; }
    public int DeductionTypeId { get; set; }
    public string? DeductionName { get; set; }
    public DeductionCalculation CalculationType { get; set; }
    public decimal Value { get; set; }
    public decimal CalculatedAmount { get; set; }
    public bool IsActive { get; set; }
}

// ============ Payroll ============
public class HrMonthlyPayrollDto
{
    public int Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? PayrollPeriod { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalBaseSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public PayrollStatus Status { get; set; }
    public string? PreparedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public List<HrPayrollItemDto> PayrollItems { get; set; } = new();
}

public class HrPayrollItemDto
{
    public int Id { get; set; }
    public int MonthlyPayrollId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? DepartmentName { get; set; }
    public string? JobTitleName { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalFixedDeductions { get; set; }
    public decimal AbsenceDeduction { get; set; }
    public decimal LateDeduction { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public bool IsPaid { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
}

// ============ Advance ============
public class HrSalaryAdvanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public DateTime RequestDate { get; set; }
    public string? Reason { get; set; }
    public AdvanceStatus Status { get; set; }
    public int DeductionMonths { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal DeductedSoFar { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}

// ============ Loan ============
public class HrEmployeeLoanDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? LoanType { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public decimal TotalRepayment { get; set; }
    public int RepaymentMonths { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public decimal PaidSoFar { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime LoanDate { get; set; }
    public LoanStatus Status { get; set; }
    public string? Notes { get; set; }
}

// ============ Bonus ============
public class HrBonusDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? BonusType { get; set; }
    public decimal Amount { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public string? Reason { get; set; }
    public BonusStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public bool IncludeInPayroll { get; set; }
    public string? Notes { get; set; }
}

// ============ Penalty ============
public class HrPenaltyDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? PenaltyType { get; set; }
    public decimal Amount { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public string? Reason { get; set; }
    public PenaltyStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public bool IncludeInPayroll { get; set; }
    public string? Notes { get; set; }
}

// ============ Promotion ============
public class HrPromotionDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public PromotionType Type { get; set; }
    public string? FromJobTitle { get; set; }
    public string? ToJobTitle { get; set; }
    public decimal? FromSalary { get; set; }
    public decimal? ToSalary { get; set; }
    public decimal? SalaryIncrease { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? DecisionNumber { get; set; }
    public HrPromotionStatus Status { get; set; }
    public string? Notes { get; set; }
    public int? FromJobTitleId { get; set; }
    public int? ToJobTitleId { get; set; }
    public int? FromJobGradeId { get; set; }
    public int? ToJobGradeId { get; set; }
    public int? FromDepartmentId { get; set; }
    public int? ToDepartmentId { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Career History ============
public class HrCareerHistoryDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EventType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ProcessedBy { get; set; }
}

// ============ Leave ============
public class HrLeaveTypeDto
{
    public int Id { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public string? LeaveTypeNameAr { get; set; }
    public string? LeaveCode { get; set; }
    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool RequiresApproval { get; set; }
    public bool RequiresDocument { get; set; }
    public bool DeductsFromSalary { get; set; }
    public decimal? DeductionPerDay { get; set; }
    public bool AllowHalfDay { get; set; }
    public bool AllowNegativeBalance { get; set; }
    public int? MaxConsecutiveDays { get; set; }
    public bool CarryForward { get; set; }
    public int? MaxCarryForwardDays { get; set; }
    public string? Color { get; set; }
    public string? ApplicableFor { get; set; }
    public bool IsActive { get; set; }
}

public class HrLeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public bool IsHalfDay { get; set; }
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
    public HrLeaveStatus Status { get; set; }
    public int? SubstituteEmployeeId { get; set; }
    public string? SubstituteName { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

public class HrLeaveBalanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public int Year { get; set; }
    public decimal TotalEntitlement { get; set; }
    public decimal CarriedForward { get; set; }
    public decimal TotalAvailable { get; set; }
    public decimal Used { get; set; }
    public decimal Pending { get; set; }
    public decimal Remaining { get; set; }
}

// ============ Holiday ============
public class HrHolidayDto
{
    public int Id { get; set; }
    public string HolidayName { get; set; } = string.Empty;
    public string? HolidayNameAr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public HolidayType Type { get; set; }
    public bool IsRecurring { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

// ============ Performance ============
public class HrPerformanceCycleDto
{
    public int Id { get; set; }
    public string CycleName { get; set; } = string.Empty;
    public string? CycleNameAr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public string? Description { get; set; }
}

public class HrPerformanceCriteriaDto
{
    public int Id { get; set; }
    public string CriteriaName { get; set; } = string.Empty;
    public string? CriteriaNameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Weight { get; set; }
    public decimal MaxScore { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class HrPerformanceReviewDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int PerformanceCycleId { get; set; }
    public string? CycleName { get; set; }
    public int ReviewerId { get; set; }
    public decimal TotalScore { get; set; }
    public decimal MaxPossibleScore { get; set; }
    public decimal Percentage { get; set; }
    public string? PerformanceRating { get; set; }
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Goals { get; set; }
    public string? ManagerComments { get; set; }
    public string? Recommendation { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public List<HrPerformanceScoreDto> Scores { get; set; } = new();
}

public class HrPerformanceScoreDto
{
    public int Id { get; set; }
    public int PerformanceReviewId { get; set; }
    public int PerformanceCriteriaId { get; set; }
    public string? CriteriaName { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal WeightedScore { get; set; }
    public string? Comments { get; set; }
}

public class HrKpiDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MeasurementUnit { get; set; }
    public decimal TargetValue { get; set; }
    public decimal ActualValue { get; set; }
    public decimal AchievementPercentage { get; set; }
    public KpiStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
}

// ============ Training ============
public class HrTrainingProgramDto
{
    public int Id { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string? ProgramNameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Provider { get; set; }
    public string? Trainer { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationHours { get; set; }
    public int MaxParticipants { get; set; }
    public decimal? Cost { get; set; }
    public TrainingStatus Status { get; set; }
    public bool IsMandatory { get; set; }
    public string? Notes { get; set; }
    public int ParticipantCount { get; set; }
    public string? SchoolName { get; set; }
}

public class HrTrainingRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int TrainingProgramId { get; set; }
    public string? ProgramName { get; set; }
    public TrainingParticipantStatus Status { get; set; }
    public decimal? Score { get; set; }
    public bool CertificateIssued { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Feedback { get; set; }
    public int? Rating { get; set; }
}

public class HrTrainingRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string RequestedTraining { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Category { get; set; }
    public decimal? EstimatedCost { get; set; }
    public DateTime? PreferredStartDate { get; set; }
    public TrainingRequestStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}

public class HrProfessionalCertificateDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string CertificateName { get; set; } = string.Empty;
    public string? IssuingBody { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public string? AttachmentPath { get; set; }
}

// ============ Disciplinary ============
public class HrViolationTypeDto
{
    public int Id { get; set; }
    public string ViolationName { get; set; } = string.Empty;
    public string? ViolationNameAr { get; set; }
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; }
    public string? DefaultAction { get; set; }
    public decimal? DefaultPenaltyAmount { get; set; }
    public int? DefaultSuspensionDays { get; set; }
    public bool IsActive { get; set; }
}

public class HrDisciplinaryActionDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int? ViolationTypeId { get; set; }
    public string? ViolationName { get; set; }
    public DateTime IncidentDate { get; set; }
    public string? IncidentDescription { get; set; }
    public DisciplinaryActionType ActionType { get; set; }
    public int? WarningLevel { get; set; }
    public int? SuspensionDays { get; set; }
    public decimal? PenaltyAmount { get; set; }
    public DisciplinaryStatus Status { get; set; }
    public string? IssuedBy { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

// ============ End of Service ============
public class HrEndOfServiceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public EndOfServiceType Type { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? LastWorkingDay { get; set; }
    public string? Reason { get; set; }
    public int TotalServiceYears { get; set; }
    public int TotalServiceMonths { get; set; }
    public decimal LastBaseSalary { get; set; }
    public decimal EndOfServiceBenefit { get; set; }
    public decimal UnusedLeaveCompensation { get; set; }
    public decimal TotalEntitlements { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal FinalSettlementAmount { get; set; }
    public bool IsSettled { get; set; }
    public EndOfServiceStatus Status { get; set; }
    public bool AllClearancesCompleted { get; set; }
    public string? Notes { get; set; }
    public string? SchoolName { get; set; }
}

// ============ Employee Request ============
public class HrEmployeeRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public EmployeeRequestType RequestType { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? AttachmentPath { get; set; }
    public EmployeeRequestStatus Status { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? Response { get; set; }
    public string? Notes { get; set; }
}

// ============ Dashboard ============
public class HrDashboardDto
{
    public int TotalActiveEmployees { get; set; }
    public int OnLeaveToday { get; set; }
    public int AbsentToday { get; set; }
    public int LateToday { get; set; }
    public decimal AverageAttendanceRate { get; set; }
    public decimal TotalPayrollThisMonth { get; set; }
    public int PendingRequests { get; set; }
    public int PendingLeaves { get; set; }
    public int PendingAdvances { get; set; }
    public int PendingOvertime { get; set; }
    public int ExpiringContracts { get; set; }
    public int ExpiringDocuments { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public Dictionary<string, int> EmployeesByType { get; set; } = new();
    public Dictionary<string, int> EmployeesByStatus { get; set; } = new();
}

// ============ HR Settings ============
public class HrSettingsDto
{
    public bool HrRequireApprovalForLeaves { get; set; }
    public bool HrAutoCalculateOvertime { get; set; }
    public bool HrAutoDeductAbsence { get; set; }
    public bool HrEnableFingerprintIntegration { get; set; }
    public bool HrEnableSelfService { get; set; }
    public int HrMaxOvertimeHoursPerMonth { get; set; }
    public decimal HrOvertimeRateMultiplier { get; set; }
    public int HrLateGracePeriodMinutes { get; set; }
    public TimeSpan HrWorkDayStart { get; set; }
    public TimeSpan HrWorkDayEnd { get; set; }
    public int HrWorkingDaysPerMonth { get; set; }
    public decimal HrAbsenceDeductionPerDay { get; set; }
    public string? HrAbsenceDeductionType { get; set; }
    public decimal HrLateDeductionPerMinute { get; set; }
    public decimal HrEarlyLeaveDeductionPerMinute { get; set; }
    public string? HrSalaryCalculationMethod { get; set; }
}
