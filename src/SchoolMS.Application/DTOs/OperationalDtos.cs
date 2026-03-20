using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

public class TeacherAssignmentDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class AttendanceDto
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public PersonType PersonType { get; set; }
    public string? BadgeCardNumber { get; set; }
    public DateTime AttendanceDate { get; set; }
    public TimeSpan Time { get; set; }
    public AttendanceType Type { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? SchoolName { get; set; }
    public bool IsAutoAbsent { get; set; }
}

public class CreateAttendanceDto
{
    public string BadgeCardNumber { get; set; } = string.Empty;
    public AttendanceType Type { get; set; }
    public int BranchId { get; set; }
}

public class BulkAttendanceItemDto
{
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public PersonType PersonType { get; set; }
    public string? BadgeCardNumber { get; set; }
}

public class BulkAttendanceSaveDto
{
    public AttendanceType Type { get; set; }
    public int BranchId { get; set; }
    public List<BulkAttendanceItemDto> Items { get; set; } = new();
}

public class AttendanceFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public AttendanceType? Type { get; set; }
    public PersonType? PersonType { get; set; }
    public int? BranchId { get; set; }
    public int? SchoolId { get; set; }
    public string? SearchValue { get; set; }
}

public class AttendanceReportDto
{
    public int TotalRecords { get; set; }
    public int CheckInCount { get; set; }
    public int CheckOutCount { get; set; }
    public int AbsentCount { get; set; }
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public int StaffCount { get; set; }
    public List<AttendanceDto> Records { get; set; } = new();
}

public class ExpenseDto
{
    public int Id { get; set; }
    public int ExpenseTypeId { get; set; }
    public string? ExpenseTypeName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class FeeInstallmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public decimal TotalAmount { get; set; }
    public int NumberOfPayments { get; set; }
    public int? DefaultPaymentPlanId { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public List<InstallmentPaymentDto> Payments { get; set; } = new();
}

public class InstallmentPaymentDto
{
    public int Id { get; set; }
    public int FeeInstallmentId { get; set; }
    public int PaymentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string? StudentName { get; set; }
    public int StudentId { get; set; }
    public string? AcademicYearName { get; set; }
    public decimal TotalInstallmentAmount { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public string? Phone { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class ExamScheduleDto
{
    public int Id { get; set; }
    public int ExamTypeId { get; set; }
    public string? ExamTypeName { get; set; }
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class WeeklyScheduleDto
{
    public int Id { get; set; }
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class StudentGradeDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int ExamTypeId { get; set; }
    public string? ExamTypeName { get; set; }
    public decimal Mark { get; set; }
    public decimal MaxMark { get; set; }
    public string? GradeLetter { get; set; }
    public int AcademicYearId { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class SalarySetupDto
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public PersonType PersonType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary => BaseSalary + Allowances - Deductions;
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public PersonType PersonType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; }
    public int SchoolId { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationTarget Target { get; set; }
    public int? TargetPersonId { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
}

public class DashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStaff { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal ExpensesThisMonth { get; set; }
    public decimal NetProfit => RevenueThisMonth - ExpensesThisMonth;
    public double AttendanceRateToday { get; set; }
    public int OverdueInstallments { get; set; }
    public List<MonthlyFinancialData> MonthlyFinancials { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class MonthlyFinancialData
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
}

public class RecentActivityDto
{
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = "fa-info-circle";
    public string Color { get; set; } = "primary";
}

public class AnalyticsDto
{
    // People
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStaff { get; set; }
    public int TotalParents { get; set; }
    public List<NameCountItem> StudentsByGrade { get; set; } = new();
    public List<NameCountItem> StudentsByBranch { get; set; } = new();

    // Finance
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalSalaries { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalPaidFees { get; set; }
    public decimal TotalUnpaidFees { get; set; }
    public List<NameAmountItem> ExpensesByType { get; set; } = new();
    public List<MonthlyFinancialData> MonthlyFinancials { get; set; } = new();

    // Academic
    public int TotalCourses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalHomework { get; set; }
    public int TotalQuizzes { get; set; }
    public double AverageGrade { get; set; }
    public List<NameValueItem> GradesBySubject { get; set; } = new();

    // Operations
    public int TotalAttendanceToday { get; set; }
    public double AttendanceRate { get; set; }
    public int TotalLeaves { get; set; }
    public int PendingLeaves { get; set; }
    public int TotalComplaints { get; set; }
    public int OpenComplaints { get; set; }
    public int TotalEvents { get; set; }
    public int UpcomingEvents { get; set; }

    // Library & Transport
    public int TotalBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalAssets { get; set; }
}

public class NameCountItem
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class NameAmountItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class NameValueItem
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? SchoolSlug { get; set; }
}

public class PortalLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int SchoolId { get; set; }
}

public class PortalLoginResultDto
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
    public string? FullName { get; set; }
    public string? UserType { get; set; }
    public int PersonId { get; set; }
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string? Username { get; set; }
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? Specialization { get; set; }
    public string? Position { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
}

public class LoginResultDto
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
}

public class DataTableRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public string? SearchValue { get; set; }
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; }

    // Additional filters
    public int? SchoolId { get; set; }
    public int? BranchId { get; set; }
    public int? AcademicYearId { get; set; }
    public int? ClassRoomId { get; set; }
    public int? GradeId { get; set; }
}

public class DataTableResponse<T>
{
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public List<T> Data { get; set; } = new();
}
