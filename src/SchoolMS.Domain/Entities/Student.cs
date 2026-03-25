namespace SchoolMS.Domain.Entities;

public class Student : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ParentPhone { get; set; }
    public string? ParentName { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public int ClassRoomId { get; set; }
    public int AcademicYearId { get; set; }
    public string? Notes { get; set; }
    public int? ParentId { get; set; }
    public string? ActiveDeviceId { get; set; }

    public virtual Parent? Parent { get; set; }
    public virtual Branch Branch { get; set; } = null!;
    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual ICollection<FeeInstallment> FeeInstallments { get; set; } = new List<FeeInstallment>();
    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
    public virtual ICollection<StudentPromotion> StudentPromotions { get; set; } = new List<StudentPromotion>();
    public virtual ICollection<StudentSubscription> StudentSubscriptions { get; set; } = new List<StudentSubscription>();
    public virtual ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    public virtual ICollection<StudentBehavior> Behaviors { get; set; } = new List<StudentBehavior>();
    public virtual ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
}
