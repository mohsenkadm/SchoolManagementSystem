using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<HrEmployee> _staffRepo;
    private readonly IRepository<Parent> _parentRepo;
    private readonly IRepository<FeeInstallment> _feeRepo;
    private readonly IRepository<InstallmentPayment> _payRepo;
    private readonly IRepository<Expense> _expenseRepo;
    private readonly IRepository<SalarySetup> _salaryRepo;
    private readonly IRepository<Attendance> _attendanceRepo;
    private readonly IRepository<Course> _courseRepo;
    private readonly IRepository<Subject> _subjectRepo;
    private readonly IRepository<Homework> _homeworkRepo;
    private readonly IRepository<QuizGroup> _quizRepo;
    private readonly IRepository<StudentGrade> _gradeRepo;
    private readonly IRepository<LeaveRequest> _leaveRepo;
    private readonly IRepository<Complaint> _complaintRepo;
    private readonly IRepository<SchoolEvent> _eventRepo;
    private readonly IRepository<LibraryBook> _bookRepo;
    private readonly IRepository<TransportRoute> _routeRepo;
    private readonly IRepository<Asset> _assetRepo;
    private readonly IRepository<Grade> _gradeDefRepo;
    private readonly IRepository<Branch> _branchRepo;
    private readonly IRepository<ExpenseType> _expTypeRepo;

    public AnalyticsService(
        IRepository<Student> studentRepo, IRepository<Teacher> teacherRepo,
        IRepository<HrEmployee> staffRepo, IRepository<Parent> parentRepo,
        IRepository<FeeInstallment> feeRepo, IRepository<InstallmentPayment> payRepo,
        IRepository<Expense> expenseRepo, IRepository<SalarySetup> salaryRepo,
        IRepository<Attendance> attendanceRepo, IRepository<Course> courseRepo,
        IRepository<Subject> subjectRepo, IRepository<Homework> homeworkRepo,
        IRepository<QuizGroup> quizRepo, IRepository<StudentGrade> gradeRepo,
        IRepository<LeaveRequest> leaveRepo, IRepository<Complaint> complaintRepo,
        IRepository<SchoolEvent> eventRepo, IRepository<LibraryBook> bookRepo,
        IRepository<TransportRoute> routeRepo, IRepository<Asset> assetRepo,
        IRepository<Grade> gradeDefRepo, IRepository<Branch> branchRepo,
        IRepository<ExpenseType> expTypeRepo)
    {
        _studentRepo = studentRepo; _teacherRepo = teacherRepo;
        _staffRepo = staffRepo; _parentRepo = parentRepo;
        _feeRepo = feeRepo; _payRepo = payRepo;
        _expenseRepo = expenseRepo; _salaryRepo = salaryRepo;
        _attendanceRepo = attendanceRepo; _courseRepo = courseRepo;
        _subjectRepo = subjectRepo; _homeworkRepo = homeworkRepo;
        _quizRepo = quizRepo; _gradeRepo = gradeRepo;
        _leaveRepo = leaveRepo; _complaintRepo = complaintRepo;
        _eventRepo = eventRepo; _bookRepo = bookRepo;
        _routeRepo = routeRepo; _assetRepo = assetRepo;
        _gradeDefRepo = gradeDefRepo; _branchRepo = branchRepo;
        _expTypeRepo = expTypeRepo;
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync(int? schoolId = null, int? branchId = null)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var dto = new AnalyticsDto();

        // People
        var studentsQ = _studentRepo.Query().AsQueryable();
        if (schoolId.HasValue) studentsQ = studentsQ.Where(s => s.SchoolId == schoolId.Value);
        if (branchId.HasValue) studentsQ = studentsQ.Where(s => s.BranchId == branchId.Value);

        var teachersQ = _teacherRepo.Query().AsQueryable();
        if (schoolId.HasValue) teachersQ = teachersQ.Where(t => t.SchoolId == schoolId.Value);
        if (branchId.HasValue) teachersQ = teachersQ.Where(t => t.BranchId == branchId.Value);

        var staffQ = _staffRepo.Query().AsQueryable();
        if (schoolId.HasValue) staffQ = staffQ.Where(s => s.SchoolId == schoolId.Value);
        if (branchId.HasValue) staffQ = staffQ.Where(s => s.BranchId == branchId.Value);

        var parentsQ = _parentRepo.Query().AsQueryable();
        if (schoolId.HasValue) parentsQ = parentsQ.Where(p => p.SchoolId == schoolId.Value);

        dto.TotalStudents = await studentsQ.CountAsync();
        dto.TotalTeachers = await teachersQ.CountAsync();
        dto.TotalStaff = await staffQ.CountAsync();
        dto.TotalParents = await parentsQ.CountAsync();

        dto.StudentsByGrade = await studentsQ
            .Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .GroupBy(s => s.ClassRoom.Grade.GradeName)
            .Select(g => new NameCountItem { Name = g.Key ?? "N/A", Count = g.Count() })
            .OrderByDescending(x => x.Count).ToListAsync();

        dto.StudentsByBranch = await studentsQ
            .Include(s => s.Branch)
            .GroupBy(s => s.Branch.Name)
            .Select(g => new NameCountItem { Name = g.Key ?? "N/A", Count = g.Count() })
            .OrderByDescending(x => x.Count).ToListAsync();

        // Finance
        var payments = _payRepo.Query().AsQueryable();
        if (schoolId.HasValue) payments = payments.Where(p => p.SchoolId == schoolId.Value);

        var feesQ = _feeRepo.Query().AsQueryable();
        if (schoolId.HasValue) feesQ = feesQ.Where(f => f.SchoolId == schoolId.Value);

        var expensesQ = _expenseRepo.Query().AsQueryable();
        if (schoolId.HasValue) expensesQ = expensesQ.Where(e => e.SchoolId == schoolId.Value);
        if (branchId.HasValue) expensesQ = expensesQ.Where(e => e.BranchId == branchId.Value);

        var salariesQ = _salaryRepo.Query().AsQueryable();
        if (schoolId.HasValue) salariesQ = salariesQ.Where(s => s.SchoolId == schoolId.Value);

        dto.TotalPaidFees = await payments.Where(p => p.Status == PaymentStatus.Paid).SumAsync(p => p.Amount);
        dto.TotalFees = await feesQ.SumAsync(f => f.TotalAmount);
        dto.TotalUnpaidFees = dto.TotalFees - dto.TotalPaidFees;
        dto.TotalExpenses = await expensesQ.SumAsync(e => e.Amount);
        dto.TotalSalaries = await salariesQ.SumAsync(s => s.BaseSalary + s.Allowances - s.Deductions);
        dto.TotalRevenue = dto.TotalPaidFees;

        dto.ExpensesByType = await expensesQ
            .Include(e => e.ExpenseType)
            .GroupBy(e => e.ExpenseType.TypeName)
            .Select(g => new NameAmountItem { Name = g.Key ?? "Other", Amount = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Amount).ToListAsync();

        // Monthly financials (last 6 months)
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);
            var rev = await payments.Where(p => p.PaidDate >= monthStart && p.PaidDate < monthEnd).SumAsync(p => p.Amount);
            var exp = await expensesQ.Where(e => e.Date >= monthStart && e.Date < monthEnd).SumAsync(e => e.Amount);
            dto.MonthlyFinancials.Add(new MonthlyFinancialData { Month = monthStart.ToString("MMM yyyy"), Revenue = rev, Expenses = exp });
        }

        // Academic
        var coursesQ = _courseRepo.Query().AsQueryable();
        if (schoolId.HasValue) coursesQ = coursesQ.Where(c => c.SchoolId == schoolId.Value);

        var subjectsQ = _subjectRepo.Query().AsQueryable();
        if (schoolId.HasValue) subjectsQ = subjectsQ.Where(s => s.SchoolId == schoolId.Value);

        var homeworkQ = _homeworkRepo.Query().AsQueryable();
        if (schoolId.HasValue) homeworkQ = homeworkQ.Where(h => h.SchoolId == schoolId.Value);

        var quizzesQ = _quizRepo.Query().AsQueryable();
        if (schoolId.HasValue) quizzesQ = quizzesQ.Where(q => q.SchoolId == schoolId.Value);

        dto.TotalCourses = await coursesQ.CountAsync();
        dto.TotalSubjects = await subjectsQ.CountAsync();
        dto.TotalHomework = await homeworkQ.CountAsync();
        dto.TotalQuizzes = await quizzesQ.CountAsync();

        var gradesQ = _gradeRepo.Query().Where(g => g.MaxMark > 0);
        if (schoolId.HasValue) gradesQ = gradesQ.Where(g => g.SchoolId == schoolId.Value);
        var grades = await gradesQ.ToListAsync();
        dto.AverageGrade = grades.Count > 0 ? Math.Round(grades.Average(g => (double)(g.Mark / g.MaxMark * 100)), 1) : 0;

        var gradesBySubjectQ = _gradeRepo.Query().Include(g => g.Subject).Where(g => g.MaxMark > 0);
        if (schoolId.HasValue) gradesBySubjectQ = gradesBySubjectQ.Where(g => g.SchoolId == schoolId.Value);
        dto.GradesBySubject = await gradesBySubjectQ
            .GroupBy(g => g.Subject.SubjectName)
            .Select(g => new NameValueItem { Name = g.Key ?? "N/A", Value = Math.Round(g.Average(x => (double)(x.Mark / x.MaxMark * 100)), 1) })
            .OrderByDescending(x => x.Value).ToListAsync();

        // Operations
        var attendanceQ = _attendanceRepo.Query().Where(a => a.AttendanceDate == today && a.Type == AttendanceType.CheckIn);
        if (schoolId.HasValue) attendanceQ = attendanceQ.Where(a => a.SchoolId == schoolId.Value);
        if (branchId.HasValue) attendanceQ = attendanceQ.Where(a => a.BranchId == branchId.Value);
        dto.TotalAttendanceToday = await attendanceQ.CountAsync();
        var totalPeople = dto.TotalStudents + dto.TotalTeachers + dto.TotalStaff;
        dto.AttendanceRate = totalPeople > 0 ? Math.Round((double)dto.TotalAttendanceToday / totalPeople * 100, 1) : 0;

        var leavesQ = _leaveRepo.Query().AsQueryable();
        if (schoolId.HasValue) leavesQ = leavesQ.Where(l => l.SchoolId == schoolId.Value);
        dto.TotalLeaves = await leavesQ.CountAsync();
        dto.PendingLeaves = await leavesQ.Where(l => l.Status == LeaveStatus.Pending).CountAsync();

        var complaintsQ = _complaintRepo.Query().AsQueryable();
        if (schoolId.HasValue) complaintsQ = complaintsQ.Where(c => c.SchoolId == schoolId.Value);
        dto.TotalComplaints = await complaintsQ.CountAsync();
        dto.OpenComplaints = await complaintsQ.Where(c => c.Status == ComplaintStatus.Open || c.Status == ComplaintStatus.InProgress).CountAsync();

        var eventsQ = _eventRepo.Query().AsQueryable();
        if (schoolId.HasValue) eventsQ = eventsQ.Where(e => e.SchoolId == schoolId.Value);
        dto.TotalEvents = await eventsQ.CountAsync();
        dto.UpcomingEvents = await eventsQ.Where(e => e.StartDate > today).CountAsync();

        // Library & Transport
        var booksQ = _bookRepo.Query().AsQueryable();
        if (schoolId.HasValue) booksQ = booksQ.Where(b => b.SchoolId == schoolId.Value);
        if (branchId.HasValue) booksQ = booksQ.Where(b => b.BranchId == branchId.Value);
        dto.TotalBooks = await booksQ.SumAsync(b => b.TotalCopies);
        dto.BorrowedBooks = dto.TotalBooks - await booksQ.SumAsync(b => b.AvailableCopies);

        var routesQ = _routeRepo.Query().AsQueryable();
        if (schoolId.HasValue) routesQ = routesQ.Where(r => r.SchoolId == schoolId.Value);
        if (branchId.HasValue) routesQ = routesQ.Where(r => r.BranchId == branchId.Value);
        dto.TotalRoutes = await routesQ.CountAsync();

        var assetsQ = _assetRepo.Query().AsQueryable();
        if (schoolId.HasValue) assetsQ = assetsQ.Where(a => a.SchoolId == schoolId.Value);
        if (branchId.HasValue) assetsQ = assetsQ.Where(a => a.BranchId == branchId.Value);
        dto.TotalAssets = await assetsQ.CountAsync();

        return dto;
    }

    public async Task<byte[]> ExportAnalyticsToExcelAsync(int? schoolId = null, int? branchId = null)
    {
        var data = await GetAnalyticsAsync(schoolId, branchId);
        using var workbook = new ClosedXML.Excel.XLWorkbook();

        // Summary Sheet
        var ws = workbook.Worksheets.Add("Summary");
        ws.Cell(1, 1).Value = "Metric"; ws.Cell(1, 2).Value = "Value";
        ws.Range("A1:B1").Style.Font.Bold = true;
        var rows = new (string, object)[]
        {
            ("Total Students", data.TotalStudents), ("Total Teachers", data.TotalTeachers),
            ("Total Staff", data.TotalStaff), ("Total Parents", data.TotalParents),
            ("Total Fees", data.TotalFees), ("Paid Fees", data.TotalPaidFees),
            ("Unpaid Fees", data.TotalUnpaidFees), ("Total Expenses", data.TotalExpenses),
            ("Total Salaries", data.TotalSalaries), ("Total Courses", data.TotalCourses),
            ("Total Subjects", data.TotalSubjects), ("Average Grade %", data.AverageGrade),
            ("Attendance Rate %", data.AttendanceRate), ("Pending Leaves", data.PendingLeaves),
            ("Open Complaints", data.OpenComplaints), ("Upcoming Events", data.UpcomingEvents),
            ("Total Books", data.TotalBooks), ("Borrowed Books", data.BorrowedBooks),
            ("Total Routes", data.TotalRoutes), ("Total Assets", data.TotalAssets)
        };
        for (int i = 0; i < rows.Length; i++)
        {
            ws.Cell(i + 2, 1).Value = rows[i].Item1;
            ws.Cell(i + 2, 2).SetValue(rows[i].Item2?.ToString() ?? "");
        }
        ws.Columns().AdjustToContents();

        // Students by Grade
        var ws2 = workbook.Worksheets.Add("Students by Grade");
        ws2.Cell(1, 1).Value = "Grade"; ws2.Cell(1, 2).Value = "Count";
        ws2.Range("A1:B1").Style.Font.Bold = true;
        for (int i = 0; i < data.StudentsByGrade.Count; i++)
        {
            ws2.Cell(i + 2, 1).Value = data.StudentsByGrade[i].Name;
            ws2.Cell(i + 2, 2).Value = data.StudentsByGrade[i].Count;
        }
        ws2.Columns().AdjustToContents();

        // Expenses by Type
        var ws3 = workbook.Worksheets.Add("Expenses by Type");
        ws3.Cell(1, 1).Value = "Type"; ws3.Cell(1, 2).Value = "Amount";
        ws3.Range("A1:B1").Style.Font.Bold = true;
        for (int i = 0; i < data.ExpensesByType.Count; i++)
        {
            ws3.Cell(i + 2, 1).Value = data.ExpensesByType[i].Name;
            ws3.Cell(i + 2, 2).Value = data.ExpensesByType[i].Amount;
        }
        ws3.Columns().AdjustToContents();

        // Monthly Financials
        var ws4 = workbook.Worksheets.Add("Monthly Financials");
        ws4.Cell(1, 1).Value = "Month"; ws4.Cell(1, 2).Value = "Revenue"; ws4.Cell(1, 3).Value = "Expenses";
        ws4.Range("A1:C1").Style.Font.Bold = true;
        for (int i = 0; i < data.MonthlyFinancials.Count; i++)
        {
            ws4.Cell(i + 2, 1).Value = data.MonthlyFinancials[i].Month;
            ws4.Cell(i + 2, 2).Value = data.MonthlyFinancials[i].Revenue;
            ws4.Cell(i + 2, 3).Value = data.MonthlyFinancials[i].Expenses;
        }
        ws4.Columns().AdjustToContents();

        // Grades by Subject
        var ws5 = workbook.Worksheets.Add("Grades by Subject");
        ws5.Cell(1, 1).Value = "Subject"; ws5.Cell(1, 2).Value = "Average %";
        ws5.Range("A1:B1").Style.Font.Bold = true;
        for (int i = 0; i < data.GradesBySubject.Count; i++)
        {
            ws5.Cell(i + 2, 1).Value = data.GradesBySubject[i].Name;
            ws5.Cell(i + 2, 2).Value = data.GradesBySubject[i].Value;
        }
        ws5.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
