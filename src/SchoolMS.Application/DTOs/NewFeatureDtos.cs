using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

// ===== Parent =====
public class ParentDto
{
    public int Id { get; set; }
    public string FatherName { get; set; } = string.Empty;
    public string? FatherPhone { get; set; }
    public string? FatherEmail { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherPhone { get; set; }
    public string? MotherEmail { get; set; }
    public string? MotherOccupation { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianRelation { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public int ChildrenCount { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? BranchName { get; set; }
    public List<ParentStudentInfo> Students { get; set; } = new();
    public List<int> StudentIds { get; set; } = new();
}

public class ParentStudentInfo
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
}

// ===== School Event =====
public class SchoolEventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public EventType EventCategory { get; set; }
    public string? Color { get; set; }
    public bool IsAllDay { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? OrganizerName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Announcement =====
public class AnnouncementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public AnnouncementPriority Priority { get; set; }
    public AnnouncementTarget Target { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Student Document =====
public class StudentDocumentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    public string? Notes { get; set; }
}

// ===== Student Behavior =====
public class StudentBehaviorDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public BehaviorType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public string? ActionTaken { get; set; }
    public string? RecordedBy { get; set; }
    public DateTime IncidentDate { get; set; }
    public bool NotifyParent { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
}

// ===== Health Record =====
public class HealthRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime RecordDate { get; set; }
    public string? RecordType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DoctorName { get; set; }
    public string? Prescription { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public bool NotifyParent { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
}

// ===== Complaint =====
public class ComplaintDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string? PersonName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ComplaintCategory Category { get; set; }
    public ComplaintPriority Priority { get; set; }
    public ComplaintStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Visitor =====
public class VisitorDto
{
    public int Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NationalId { get; set; }
    public string? Purpose { get; set; }
    public string? VisitingPerson { get; set; }
    public string? VisitingDepartment { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public VisitorStatus Status { get; set; }
    public string? Notes { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Library =====
public class LibraryBookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public string? ShelfLocation { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? Barcode { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class BookBorrowDto
{
    public int Id { get; set; }
    public int LibraryBookId { get; set; }
    public string? BookTitle { get; set; }
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public PersonType PersonType { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; }
    public decimal? FineAmount { get; set; }
}

// ===== Transport =====
public class TransportRouteDto
{
    public int Id { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? BusNumber { get; set; }
    public int Capacity { get; set; }
    public int CurrentPassengers { get; set; }
    public decimal MonthlyFee { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool IsActive { get; set; }
    public int StopCount { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Assets =====
public class AssetCategoryDto
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int AssetCount { get; set; }
}

public class AssetDto
{
    public int Id { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public int AssetCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? AssetCode { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentValue { get; set; }
    public string? Condition { get; set; }
    public string? Location { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public AssetStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

// ===== Audit Log =====
public class AuditLogDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string? PageName { get; set; }
}
