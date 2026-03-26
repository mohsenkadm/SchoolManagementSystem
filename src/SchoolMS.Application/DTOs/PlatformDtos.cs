namespace SchoolMS.Application.DTOs;

public class AddExtraStorageRequest
{
    public int SubscriptionId { get; set; }
    public decimal ExtraGB { get; set; }
    public decimal PricePerGB { get; set; }
}

public class SchoolDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool OnlinePlatformEnabled { get; set; }
    public decimal DefaultTeacherCommissionRate { get; set; }
    public bool IsHrModuleEnabled { get; set; }
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
    public string? OneSignalAppId { get; set; }
    public string? OneSignalApiKey { get; set; }
    public int BranchCount { get; set; }
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public string? CurrentPlan { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SchoolCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiryDate { get; set; }
    public decimal DefaultTeacherCommissionRate { get; set; }
    public bool IsHrModuleEnabled { get; set; }
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
    public string? OneSignalAppId { get; set; }
    public string? OneSignalApiKey { get; set; }
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminFullName { get; set; } = string.Empty;
    public int? SubscriptionPlanId { get; set; }
}

public class SchoolUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal DefaultTeacherCommissionRate { get; set; }
    public bool IsHrModuleEnabled { get; set; }
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
    public string? OneSignalAppId { get; set; }
    public string? OneSignalApiKey { get; set; }
}

public class SubscriptionPlanDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxUsers { get; set; }
    public int MaxStudents { get; set; }
    public int DurationMonths { get; set; }
    public bool IncludesHrModule { get; set; }
    public bool IncludesCourses { get; set; }
    public bool IncludesLiveStream { get; set; }
    public decimal StorageLimitGB { get; set; }
    public int ActiveSchoolCount { get; set; }
}

public class SchoolSubscriptionDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int SystemSubscriptionPlanId { get; set; }
    public string? PlanName { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public decimal StorageLimitGB { get; set; }
    public decimal ExtraStorageGB { get; set; }
    public decimal ExtraStoragePrice { get; set; }
    public decimal UsedStorageGB { get; set; }
    public decimal TotalStorageGB => StorageLimitGB + ExtraStorageGB;
}

public class StorageRequestDto
{
    public int Id { get; set; }
    public int SchoolSubscriptionId { get; set; }
    public int? StoragePlanId { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? PlanName { get; set; }
    public string? StoragePlanName { get; set; }
    public decimal RequestedGB { get; set; }
    public decimal PricePerGB { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public bool IsApproved { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StoragePlanDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal StorageGB { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int RequestCount { get; set; }
}

public class StorageQuotaDto
{
    public decimal StorageLimitGB { get; set; }
    public decimal ExtraStorageGB { get; set; }
    public decimal UsedStorageGB { get; set; }
    public decimal TotalStorageGB => StorageLimitGB + ExtraStorageGB;
    public decimal AvailableGB => TotalStorageGB - UsedStorageGB;
    public bool HasSpace(long fileSizeBytes) => AvailableGB > (decimal)fileSizeBytes / (1024m * 1024m * 1024m);
}

public class PlatformDashboardDto
{
    public int TotalSchools { get; set; }
    public int ActiveSchools { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiringSoon { get; set; }
    public List<SchoolDto> RecentSchools { get; set; } = [];
}
