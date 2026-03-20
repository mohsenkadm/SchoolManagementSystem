using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrEmployee : BaseEntity
{
    // Personal Information
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

    // Contact Information
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Email { get; set; }
    public string? PersonalEmail { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelation { get; set; }

    // Employment Information
    public int DepartmentId { get; set; }
    public int JobTitleId { get; set; }
    public int? JobGradeId { get; set; }
    public int? JobGradeStepId { get; set; }
    public int BranchId { get; set; }
    public int? WorkShiftId { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public EmployeeCategory Category { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? ProbationEndDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public HrEmployeeStatus Status { get; set; }
    public int? DirectManagerId { get; set; }
    public int? ReportsToEmployeeId { get; set; }

    // Badge & Fingerprint
    public string? BadgeCardNumber { get; set; }
    public string? FingerprintId { get; set; }
    public string? QrCode { get; set; }
    public string? BarcodeNumber { get; set; }

    // Authentication
    public string? UserId { get; set; }
    public string? Username { get; set; }

    // Financial Information
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? IBAN { get; set; }
    public string? Currency { get; set; }
    public string? TaxNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }

    // Education
    public string? HighestQualification { get; set; }
    public string? University { get; set; }
    public string? Major { get; set; }
    public int? GraduationYear { get; set; }
    public int YearsOfExperience { get; set; }

    // Notes
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }

    // Navigation Properties
    public virtual HrDepartment Department { get; set; } = null!;
    public virtual HrJobTitle JobTitle { get; set; } = null!;
    public virtual HrJobGrade? JobGrade { get; set; }
    public virtual HrJobGradeStep? JobGradeStep { get; set; }
    public virtual HrWorkShift? WorkShift { get; set; }
    public virtual HrEmployee? DirectManager { get; set; }
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<HrEmployeeContract> Contracts { get; set; } = new List<HrEmployeeContract>();
    public virtual ICollection<HrEmployeeDocument> Documents { get; set; } = new List<HrEmployeeDocument>();
    public virtual ICollection<HrFingerprintRecord> FingerprintRecords { get; set; } = new List<HrFingerprintRecord>();
    public virtual ICollection<HrSalaryDetail> SalaryDetails { get; set; } = new List<HrSalaryDetail>();
    public virtual ICollection<HrLeaveRequest> LeaveRequests { get; set; } = new List<HrLeaveRequest>();
    public virtual ICollection<HrPromotion> Promotions { get; set; } = new List<HrPromotion>();
    public virtual ICollection<HrDisciplinaryAction> DisciplinaryActions { get; set; } = new List<HrDisciplinaryAction>();
    public virtual ICollection<HrPerformanceReview> PerformanceReviews { get; set; } = new List<HrPerformanceReview>();
    public virtual ICollection<HrTrainingRecord> TrainingRecords { get; set; } = new List<HrTrainingRecord>();
}
