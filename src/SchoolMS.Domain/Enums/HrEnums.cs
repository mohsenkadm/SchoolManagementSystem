namespace SchoolMS.Domain.Enums;

public enum EmployeeType { FullTime, PartTime, Contract, Temporary, Intern }
public enum EmployeeCategory { Teacher, Admin, Support, Maintenance, Security, Driver, Librarian, Nurse, Other }
public enum HrEmployeeStatus { Active, OnProbation, OnLeave, Suspended, Resigned, Terminated, Retired, Deceased }
public enum ContractType { Permanent, FixedTerm, Temporary, Probation, Freelance }
public enum ContractStatus { Active, Expired, Terminated, Renewed, PendingRenewal }
public enum FingerprintType { CheckIn, CheckOut }
public enum FingerprintSource { Device, Manual, MobileApp, QrCode, BadgeCard, FaceRecognition }
public enum DeviceStatus { Online, Offline, Maintenance, Disconnected }
public enum DailyAttendanceStatus
{
    Present, Absent, Late, EarlyLeave, LateAndEarlyLeave,
    HalfDay, OnLeave, Holiday, Weekend, BusinessTrip, WorkFromHome,
    Suspended, ExcusedAbsence
}
public enum OvertimeStatus { Pending, Approved, Rejected }
public enum SalaryType { Monthly, Daily, Hourly }
public enum AllowanceCalculation { Fixed, PercentageOfBasic, PercentageOfGross }
public enum DeductionCalculation { Fixed, PercentageOfBasic, PercentageOfGross }
public enum PayrollStatus { Draft, Calculated, Approved, Paid, Locked }
public enum AdvanceStatus { Pending, Approved, Rejected, Paid, FullyDeducted }
public enum LoanStatus { Active, FullyPaid, Defaulted, WrittenOff }
public enum BonusStatus { Pending, Approved, Rejected, Paid }
public enum PenaltyStatus { Pending, Approved, Rejected, Deducted }
public enum HrPromotionStatus { Pending, Approved, Rejected, Effective }
public enum PromotionType { Promotion, GradeUpgrade, StepIncrement, TitleChange, DepartmentTransfer, BranchTransfer, Demotion }
public enum HrLeaveStatus { Pending, ApprovedByManager, ApprovedByHR, Rejected, Cancelled }
public enum HolidayType { National, Religious, School, Custom }
public enum CycleStatus { Planning, Active, Completed, Archived }
public enum ReviewStatus { Draft, PendingEmployee, PendingManager, PendingHR, Completed }
public enum KpiStatus { OnTrack, Behind, Achieved, Exceeded, NotStarted }
public enum TrainingStatus { Planned, InProgress, Completed, Cancelled }
public enum TrainingParticipantStatus { Enrolled, Attended, Completed, Failed, NoShow }
public enum TrainingRequestStatus { Pending, Approved, Rejected, Scheduled }
public enum ViolationSeverity { Minor, Moderate, Major, Critical }
public enum DisciplinaryActionType { VerbalWarning, WrittenWarning, FinalWarning, Suspension, Demotion, Termination, ProbationExtension }
public enum DisciplinaryStatus { Issued, Acknowledged, Appealed, Resolved, Expired }
public enum EndOfServiceType { Resignation, Termination, Retirement, ContractEnd, Death, MedicalDisability, MutualAgreement }
public enum EndOfServiceStatus { Requested, InProgress, ClearancePhase, Approved, Settled, Completed }
public enum EmployeeRequestType { SalarySlipReprint, EmploymentLetter, ExperienceLetter, SalaryCertificate, DataUpdate, ShiftChange, TransferRequest, Complaint }
public enum EmployeeRequestStatus { Pending, InProgress, Completed, Rejected }
