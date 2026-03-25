using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;

namespace SchoolMS.Infrastructure.Data;

public class SchoolDbContext : IdentityDbContext<ApplicationUser>
{
    // Public property so EF Core correctly parameterizes the query filter
    // across cached model instances (evaluated per-query from the current DbContext).
    public int? CurrentSchoolId { get; set; }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        CurrentSchoolId = tenantProvider.GetCurrentSchoolId();
    }

    // Core
    public DbSet<School> Schools => Set<School>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

    // People
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();

    // Academic Structure
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
    public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();

    // Salary
    public DbSet<SalarySetup> SalarySetups => Set<SalarySetup>();
    public DbSet<MonthlySalary> MonthlySalaries => Set<MonthlySalary>();
    public DbSet<SalaryTransaction> SalaryTransactions => Set<SalaryTransaction>();

    // Expenses
    public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
    public DbSet<Expense> Expenses => Set<Expense>();

    // Fees
    public DbSet<FeeInstallment> FeeInstallments => Set<FeeInstallment>();
    public DbSet<InstallmentPayment> InstallmentPayments => Set<InstallmentPayment>();
    public DbSet<DefaultPaymentPlan> DefaultPaymentPlans => Set<DefaultPaymentPlan>();
    public DbSet<DefaultPaymentDate> DefaultPaymentDates => Set<DefaultPaymentDate>();

    // Exams & Grades
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<ExamSchedule> ExamSchedules => Set<ExamSchedule>();
    public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();

    // Schedule
    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();

    // Attendance
    public DbSet<Attendance> Attendances => Set<Attendance>();

    // Leave
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    // Homework
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<HomeworkAttachment> HomeworkAttachments => Set<HomeworkAttachment>();
    public DbSet<HomeworkComment> HomeworkComments => Set<HomeworkComment>();

    // Quizzes
    public DbSet<QuizGroup> QuizGroups => Set<QuizGroup>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();

    // Promotion
    public DbSet<StudentPromotion> StudentPromotions => Set<StudentPromotion>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();

    // Chat
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    // Carousel
    public DbSet<CarouselImage> CarouselImages => Set<CarouselImage>();

    // Teacher Earnings
    public DbSet<TeacherEarning> TeacherEarnings => Set<TeacherEarning>();

    // Online Platform
    public DbSet<OnlineSubscriptionPlan> OnlineSubscriptionPlans => Set<OnlineSubscriptionPlan>();
    public DbSet<StudentSubscription> StudentSubscriptions => Set<StudentSubscription>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoCodeUsage> PromoCodeUsages => Set<PromoCodeUsage>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseVideo> CourseVideos => Set<CourseVideo>();
    public DbSet<VideoComment> VideoComments => Set<VideoComment>();
    public DbSet<VideoRating> VideoRatings => Set<VideoRating>();
    public DbSet<VideoFavorite> VideoFavorites => Set<VideoFavorite>();
    public DbSet<LiveStream> LiveStreams => Set<LiveStream>();
    public DbSet<LiveStreamComment> LiveStreamComments => Set<LiveStreamComment>();
    public DbSet<VideoLike> VideoLikes => Set<VideoLike>();
    public DbSet<VideoSeen> VideoSeens => Set<VideoSeen>();
    public DbSet<LiveStreamSeen> LiveStreamSeens => Set<LiveStreamSeen>();
    public DbSet<VideoQuizQuestion> VideoQuizQuestions => Set<VideoQuizQuestion>();
    public DbSet<VideoQuizAnswer> VideoQuizAnswers => Set<VideoQuizAnswer>();
    public DbSet<VideoNote> VideoNotes => Set<VideoNote>();

    // System
    public DbSet<SystemSubscriptionPlan> SystemSubscriptionPlans => Set<SystemSubscriptionPlan>();
    public DbSet<SchoolSubscription> SchoolSubscriptions => Set<SchoolSubscription>();
    public DbSet<StoragePlan> StoragePlans => Set<StoragePlan>();
    public DbSet<StorageRequest> StorageRequests => Set<StorageRequest>();

    // Permissions
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();

    // New Feature Modules
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SchoolEvent> SchoolEvents => Set<SchoolEvent>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<StudentBehavior> StudentBehaviors => Set<StudentBehavior>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<LibraryBook> LibraryBooks => Set<LibraryBook>();
    public DbSet<BookBorrow> BookBorrows => Set<BookBorrow>();
    public DbSet<TransportRoute> TransportRoutes => Set<TransportRoute>();
    public DbSet<TransportStop> TransportStops => Set<TransportStop>();
    public DbSet<StudentTransport> StudentTransports => Set<StudentTransport>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Asset> Assets => Set<Asset>();

    // HR Module
    public DbSet<HrDepartment> HrDepartments => Set<HrDepartment>();
    public DbSet<HrJobTitle> HrJobTitles => Set<HrJobTitle>();
    public DbSet<HrJobGrade> HrJobGrades => Set<HrJobGrade>();
    public DbSet<HrJobGradeStep> HrJobGradeSteps => Set<HrJobGradeStep>();
    public DbSet<HrEmployee> HrEmployees => Set<HrEmployee>();
    public DbSet<HrEmployeeContract> HrEmployeeContracts => Set<HrEmployeeContract>();
    public DbSet<HrEmployeeDocument> HrEmployeeDocuments => Set<HrEmployeeDocument>();
    public DbSet<HrWorkShift> HrWorkShifts => Set<HrWorkShift>();
    public DbSet<HrFingerprintDevice> HrFingerprintDevices => Set<HrFingerprintDevice>();
    public DbSet<HrFingerprintRecord> HrFingerprintRecords => Set<HrFingerprintRecord>();
    public DbSet<HrDailyAttendance> HrDailyAttendances => Set<HrDailyAttendance>();
    public DbSet<HrOvertimeRequest> HrOvertimeRequests => Set<HrOvertimeRequest>();
    public DbSet<HrSalaryDetail> HrSalaryDetails => Set<HrSalaryDetail>();
    public DbSet<HrAllowanceType> HrAllowanceTypes => Set<HrAllowanceType>();
    public DbSet<HrSalaryAllowance> HrSalaryAllowances => Set<HrSalaryAllowance>();
    public DbSet<HrDeductionType> HrDeductionTypes => Set<HrDeductionType>();
    public DbSet<HrSalaryDeduction> HrSalaryDeductions => Set<HrSalaryDeduction>();
    public DbSet<HrMonthlyPayroll> HrMonthlyPayrolls => Set<HrMonthlyPayroll>();
    public DbSet<HrPayrollItem> HrPayrollItems => Set<HrPayrollItem>();
    public DbSet<HrSalaryAdvance> HrSalaryAdvances => Set<HrSalaryAdvance>();
    public DbSet<HrAdvanceDeductionLog> HrAdvanceDeductionLogs => Set<HrAdvanceDeductionLog>();
    public DbSet<HrEmployeeLoan> HrEmployeeLoans => Set<HrEmployeeLoan>();
    public DbSet<HrLoanRepaymentLog> HrLoanRepaymentLogs => Set<HrLoanRepaymentLog>();
    public DbSet<HrBonus> HrBonuses => Set<HrBonus>();
    public DbSet<HrPenalty> HrPenalties => Set<HrPenalty>();
    public DbSet<HrPromotion> HrPromotions => Set<HrPromotion>();
    public DbSet<HrCareerHistory> HrCareerHistories => Set<HrCareerHistory>();
    public DbSet<HrLeaveType> HrLeaveTypes => Set<HrLeaveType>();
    public DbSet<HrLeaveRequest> HrLeaveRequests => Set<HrLeaveRequest>();
    public DbSet<HrLeaveBalance> HrLeaveBalances => Set<HrLeaveBalance>();
    public DbSet<HrHoliday> HrHolidays => Set<HrHoliday>();
    public DbSet<HrPerformanceCycle> HrPerformanceCycles => Set<HrPerformanceCycle>();
    public DbSet<HrPerformanceCriteria> HrPerformanceCriterias => Set<HrPerformanceCriteria>();
    public DbSet<HrPerformanceReview> HrPerformanceReviews => Set<HrPerformanceReview>();
    public DbSet<HrPerformanceScore> HrPerformanceScores => Set<HrPerformanceScore>();
    public DbSet<HrKpi> HrKpis => Set<HrKpi>();
    public DbSet<HrTrainingProgram> HrTrainingPrograms => Set<HrTrainingProgram>();
    public DbSet<HrTrainingRecord> HrTrainingRecords => Set<HrTrainingRecord>();
    public DbSet<HrTrainingRequest> HrTrainingRequests => Set<HrTrainingRequest>();
    public DbSet<HrProfessionalCertificate> HrProfessionalCertificates => Set<HrProfessionalCertificate>();
    public DbSet<HrViolationType> HrViolationTypes => Set<HrViolationType>();
    public DbSet<HrDisciplinaryAction> HrDisciplinaryActions => Set<HrDisciplinaryAction>();
    public DbSet<HrEndOfService> HrEndOfServices => Set<HrEndOfService>();
    public DbSet<HrClearanceItem> HrClearanceItems => Set<HrClearanceItem>();
    public DbSet<HrEmployeeRequest> HrEmployeeRequests => Set<HrEmployeeRequest>();

    // Registration Requests (from marketing website)
    public DbSet<SchoolRegistrationRequest> SchoolRegistrationRequests => Set<SchoolRegistrationRequest>();

    // OTP Verification
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // School inherits BaseEntity.SchoolId — ignore the self-referencing navigation
        builder.Entity<School>(e =>
        {
            e.Ignore(s => s.School);
        });

        // Set all SchoolId FKs to NoAction to prevent cascade cycles (School -> Branch -> Entity)
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.ClrType != typeof(School))
            {
                var schoolFk = entityType.GetForeignKeys()
                    .FirstOrDefault(fk => fk.Properties.Any(p => p.Name == "SchoolId"));
                if (schoolFk != null)
                    schoolFk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        // ApplicationUser.SchoolId FK — prevent cascade cycle
        // Also apply tenant isolation filter (ApplicationUser does NOT inherit BaseEntity)
        builder.Entity<ApplicationUser>(e =>
        {
            e.HasOne(u => u.School).WithMany().HasForeignKey(u => u.SchoolId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(u => u.Branch).WithMany().HasForeignKey(u => u.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasQueryFilter(u =>
                !u.IsDeleted &&
                (CurrentSchoolId == null || u.SchoolId == CurrentSchoolId));
        });

        // Global query filter for soft delete and multi-tenancy
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SchoolDbContext)
                    .GetMethod(nameof(ApplyGlobalFilters), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { builder });
            }
        }

        // StudentPromotion multiple FK to ClassRoom and AcademicYear
        builder.Entity<StudentPromotion>(e =>
        {
            e.HasOne(sp => sp.FromClassRoom).WithMany().HasForeignKey(sp => sp.FromClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sp => sp.ToClassRoom).WithMany().HasForeignKey(sp => sp.ToClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sp => sp.FromAcademicYear).WithMany().HasForeignKey(sp => sp.FromAcademicYearId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sp => sp.ToAcademicYear).WithMany().HasForeignKey(sp => sp.ToAcademicYearId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sp => sp.Student).WithMany(s => s.StudentPromotions).HasForeignKey(sp => sp.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        // Prevent cascade delete cycles
        builder.Entity<Student>(e =>
        {
            e.HasOne(s => s.Branch).WithMany(b => b.Students).HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(s => s.ClassRoom).WithMany(c => c.Students).HasForeignKey(s => s.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(s => s.AcademicYear).WithMany(a => a.Students).HasForeignKey(s => s.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<TeacherAssignment>(e =>
        {
            e.HasOne(ta => ta.Teacher).WithMany(t => t.TeacherAssignments).HasForeignKey(ta => ta.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ta => ta.ClassRoom).WithMany(c => c.TeacherAssignments).HasForeignKey(ta => ta.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ta => ta.Subject).WithMany(s => s.TeacherAssignments).HasForeignKey(ta => ta.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ta => ta.AcademicYear).WithMany(a => a.TeacherAssignments).HasForeignKey(ta => ta.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ta => ta.Branch).WithMany().HasForeignKey(ta => ta.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<ClassRoom>(e =>
        {
            e.HasOne(c => c.Grade).WithMany(g => g.ClassRooms).HasForeignKey(c => c.GradeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(c => c.Division).WithMany(d => d.ClassRooms).HasForeignKey(c => c.DivisionId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(c => c.AcademicYear).WithMany(a => a.ClassRooms).HasForeignKey(c => c.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(c => c.Branch).WithMany(b => b.ClassRooms).HasForeignKey(c => c.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Grade>(e =>
        {
            e.HasOne(g => g.Branch).WithMany().HasForeignKey(g => g.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<ExamSchedule>(e =>
        {
            e.HasOne(es => es.ExamType).WithMany(et => et.ExamSchedules).HasForeignKey(es => es.ExamTypeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(es => es.ClassRoom).WithMany(c => c.ExamSchedules).HasForeignKey(es => es.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(es => es.Subject).WithMany(s => s.ExamSchedules).HasForeignKey(es => es.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(es => es.Teacher).WithMany().HasForeignKey(es => es.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(es => es.AcademicYear).WithMany().HasForeignKey(es => es.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<WeeklySchedule>(e =>
        {
            e.HasOne(ws => ws.ClassRoom).WithMany(c => c.WeeklySchedules).HasForeignKey(ws => ws.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ws => ws.Subject).WithMany(s => s.WeeklySchedules).HasForeignKey(ws => ws.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ws => ws.Teacher).WithMany().HasForeignKey(ws => ws.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ws => ws.AcademicYear).WithMany().HasForeignKey(ws => ws.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<StudentGrade>(e =>
        {
            e.HasOne(sg => sg.Student).WithMany(s => s.StudentGrades).HasForeignKey(sg => sg.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sg => sg.Subject).WithMany(s => s.StudentGrades).HasForeignKey(sg => sg.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sg => sg.ExamType).WithMany(et => et.StudentGrades).HasForeignKey(sg => sg.ExamTypeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sg => sg.AcademicYear).WithMany().HasForeignKey(sg => sg.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Homework>(e =>
        {
            e.HasOne(h => h.Teacher).WithMany(t => t.Homeworks).HasForeignKey(h => h.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(h => h.ClassRoom).WithMany().HasForeignKey(h => h.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(h => h.Subject).WithMany().HasForeignKey(h => h.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(h => h.AcademicYear).WithMany().HasForeignKey(h => h.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HomeworkComment>(e =>
        {
            e.HasOne(hc => hc.Homework).WithMany(h => h.Comments).HasForeignKey(hc => hc.HomeworkId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(hc => hc.Student).WithMany().HasForeignKey(hc => hc.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<QuizGroup>(e =>
        {
            e.HasOne(qg => qg.ClassRoom).WithMany().HasForeignKey(qg => qg.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(qg => qg.Subject).WithMany().HasForeignKey(qg => qg.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(qg => qg.Teacher).WithMany().HasForeignKey(qg => qg.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(qg => qg.AcademicYear).WithMany().HasForeignKey(qg => qg.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<QuizAnswer>(e =>
        {
            e.HasOne(qa => qa.QuizQuestion).WithMany(qq => qq.Answers).HasForeignKey(qa => qa.QuizQuestionId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(qa => qa.Student).WithMany(s => s.QuizAnswers).HasForeignKey(qa => qa.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<FeeInstallment>(e =>
        {
            e.HasOne(fi => fi.Student).WithMany(s => s.FeeInstallments).HasForeignKey(fi => fi.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(fi => fi.AcademicYear).WithMany().HasForeignKey(fi => fi.AcademicYearId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Attendance>(e =>
        {
            e.HasOne(a => a.Branch).WithMany().HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Expense>(e =>
        {
            e.HasOne(ex => ex.Branch).WithMany().HasForeignKey(ex => ex.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ex => ex.ExpenseType).WithMany(et => et.Expenses).HasForeignKey(ex => ex.ExpenseTypeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Teacher>(e =>
        {
            e.HasOne(t => t.Branch).WithMany(b => b.Teachers).HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Grade>(e =>
        {
            e.HasOne(g => g.Division).WithMany(d => d.Grades).HasForeignKey(g => g.DivisionId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Course>(e =>
        {
            e.HasOne(c => c.Subject).WithMany().HasForeignKey(c => c.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(c => c.Teacher).WithMany().HasForeignKey(c => c.TeacherId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<LiveStream>(e =>
        {
            e.HasOne(ls => ls.Course).WithMany(c => c.LiveStreams).HasForeignKey(ls => ls.CourseId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ls => ls.Subject).WithMany().HasForeignKey(ls => ls.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ls => ls.Teacher).WithMany().HasForeignKey(ls => ls.TeacherId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<LiveStreamComment>(e =>
        {
            e.HasOne(lc => lc.LiveStream).WithMany(ls => ls.Comments).HasForeignKey(lc => lc.LiveStreamId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(lc => lc.Student).WithMany().HasForeignKey(lc => lc.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(lc => lc.Teacher).WithMany().HasForeignKey(lc => lc.TeacherId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoComment>(e =>
        {
            e.HasOne(vc => vc.CourseVideo).WithMany(cv => cv.Comments).HasForeignKey(vc => vc.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(vc => vc.Student).WithMany().HasForeignKey(vc => vc.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoRating>(e =>
        {
            e.HasOne(vr => vr.CourseVideo).WithMany(cv => cv.Ratings).HasForeignKey(vr => vr.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(vr => vr.Student).WithMany().HasForeignKey(vr => vr.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoFavorite>(e =>
        {
            e.HasOne(vf => vf.CourseVideo).WithMany(cv => cv.Favorites).HasForeignKey(vf => vf.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(vf => vf.Student).WithMany().HasForeignKey(vf => vf.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoLike>(e =>
        {
            e.HasOne(vl => vl.CourseVideo).WithMany(cv => cv.Likes).HasForeignKey(vl => vl.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(vl => vl.Student).WithMany().HasForeignKey(vl => vl.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoSeen>(e =>
        {
            e.HasOne(vs => vs.CourseVideo).WithMany(cv => cv.Seens).HasForeignKey(vs => vs.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(vs => vs.Student).WithMany().HasForeignKey(vs => vs.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<LiveStreamSeen>(e =>
        {
            e.HasOne(ls => ls.LiveStream).WithMany(l => l.Seens).HasForeignKey(ls => ls.LiveStreamId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ls => ls.Student).WithMany().HasForeignKey(ls => ls.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<StudentSubscription>(e =>
        {
            e.HasOne(ss => ss.Student).WithMany(s => s.StudentSubscriptions).HasForeignKey(ss => ss.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ss => ss.OnlineSubscriptionPlan).WithMany(p => p.StudentSubscriptions).HasForeignKey(ss => ss.OnlineSubscriptionPlanId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<PromoCode>(e =>
        {
            e.HasIndex(p => p.Code).IsUnique();
        });

        builder.Entity<PromoCodeUsage>(e =>
        {
            e.HasOne(u => u.PromoCode).WithMany(p => p.Usages).HasForeignKey(u => u.PromoCodeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(u => u.Student).WithMany().HasForeignKey(u => u.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(u => u.StudentSubscription).WithMany().HasForeignKey(u => u.StudentSubscriptionId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<TeacherEarning>(e =>
        {
            e.HasOne(te => te.Teacher).WithMany().HasForeignKey(te => te.TeacherId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(te => te.Course).WithMany().HasForeignKey(te => te.CourseId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(te => te.StudentSubscription).WithMany().HasForeignKey(te => te.StudentSubscriptionId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<StorageRequest>(e =>
        {
            e.HasOne(sr => sr.SchoolSubscription).WithMany(ss => ss.StorageRequests).HasForeignKey(sr => sr.SchoolSubscriptionId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(sr => sr.StoragePlan).WithMany(sp => sp.StorageRequests).HasForeignKey(sr => sr.StoragePlanId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<ChatRoom>(e =>
        {
            e.HasOne(cr => cr.Branch).WithMany().HasForeignKey(cr => cr.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(cr => cr.ClassRoom).WithMany().HasForeignKey(cr => cr.ClassRoomId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(cr => cr.Subject).WithMany().HasForeignKey(cr => cr.SubjectId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(cr => cr.Teacher).WithMany().HasForeignKey(cr => cr.TeacherId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<UserPermission>(e =>
        {
            e.HasOne(up => up.User).WithMany(u => u.UserPermissions).HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(up => up.Permission).WithMany(p => p.UserPermissions).HasForeignKey(up => up.PermissionId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<UserBranch>(e =>
        {
            e.HasOne(ub => ub.User).WithMany(u => u.UserBranches).HasForeignKey(ub => ub.UserId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(ub => ub.Branch).WithMany().HasForeignKey(ub => ub.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        // New entity relationships
        builder.Entity<Student>(e2 =>
        {
            e2.HasOne(s => s.Parent).WithMany(p => p.Children).HasForeignKey(s => s.ParentId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<StudentDocument>(e =>
        {
            e.HasOne(sd => sd.Student).WithMany(s => s.Documents).HasForeignKey(sd => sd.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<StudentBehavior>(e =>
        {
            e.HasOne(sb => sb.Student).WithMany(s => s.Behaviors).HasForeignKey(sb => sb.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HealthRecord>(e =>
        {
            e.HasOne(hr => hr.Student).WithMany(s => s.HealthRecords).HasForeignKey(hr => hr.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<SchoolEvent>(e =>
        {
            e.HasOne(se => se.Branch).WithMany().HasForeignKey(se => se.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Announcement>(e =>
        {
            e.HasOne(a => a.Branch).WithMany().HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Grade).WithMany().HasForeignKey(a => a.GradeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Visitor>(e =>
        {
            e.HasOne(v => v.Branch).WithMany().HasForeignKey(v => v.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<LibraryBook>(e =>
        {
            e.HasOne(lb => lb.Branch).WithMany().HasForeignKey(lb => lb.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<BookBorrow>(e =>
        {
            e.HasOne(bb => bb.LibraryBook).WithMany(lb => lb.Borrows).HasForeignKey(bb => bb.LibraryBookId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<TransportRoute>(e =>
        {
            e.HasOne(tr => tr.Branch).WithMany().HasForeignKey(tr => tr.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<TransportStop>(e =>
        {
            e.HasOne(ts => ts.TransportRoute).WithMany(tr => tr.Stops).HasForeignKey(ts => ts.TransportRouteId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<StudentTransport>(e =>
        {
            e.HasOne(st => st.Student).WithMany().HasForeignKey(st => st.StudentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(st => st.TransportRoute).WithMany(tr => tr.StudentTransports).HasForeignKey(st => st.TransportRouteId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(st => st.TransportStop).WithMany().HasForeignKey(st => st.TransportStopId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Asset>(e =>
        {
            e.HasOne(a => a.AssetCategory).WithMany(ac => ac.Assets).HasForeignKey(a => a.AssetCategoryId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Branch).WithMany().HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Complaint>(e =>
        {
            e.HasIndex(c => c.TicketNumber).IsUnique();
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasIndex(a => new { a.SchoolId, a.Timestamp });
            e.HasIndex(a => a.UserId);
        });

        // SchoolRegistrationRequest (not tenant-scoped)
        builder.Entity<SchoolRegistrationRequest>(e =>
        {
            e.HasOne(r => r.CreatedSchool).WithMany().HasForeignKey(r => r.CreatedSchoolId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(r => r.Status);
            e.HasIndex(r => r.SubmittedAt);
        });

        // OTP Verification (not tenant-scoped)
        builder.Entity<OtpVerification>(e =>
        {
            e.HasIndex(o => o.Phone);
            e.HasIndex(o => new { o.Phone, o.IsUsed, o.CreatedAt });
        });

        // ============================================================
        // HR MODULE RELATIONSHIPS
        // ============================================================

        builder.Entity<HrDepartment>(e =>
        {
            e.HasOne(d => d.ParentDepartment).WithMany(d => d.SubDepartments).HasForeignKey(d => d.ParentDepartmentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(d => d.Branch).WithMany().HasForeignKey(d => d.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrJobTitle>(e =>
        {
            e.HasOne(j => j.Department).WithMany().HasForeignKey(j => j.DepartmentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrJobGradeStep>(e =>
        {
            e.HasOne(s => s.JobGrade).WithMany(g => g.Steps).HasForeignKey(s => s.JobGradeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrEmployee>(e =>
        {
            e.HasOne(emp => emp.Department).WithMany(d => d.Employees).HasForeignKey(emp => emp.DepartmentId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.JobTitle).WithMany().HasForeignKey(emp => emp.JobTitleId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.JobGrade).WithMany().HasForeignKey(emp => emp.JobGradeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.JobGradeStep).WithMany().HasForeignKey(emp => emp.JobGradeStepId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.Branch).WithMany(b => b.StaffMembers).HasForeignKey(emp => emp.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.WorkShift).WithMany(ws => ws.Employees).HasForeignKey(emp => emp.WorkShiftId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(emp => emp.DirectManager).WithMany().HasForeignKey(emp => emp.DirectManagerId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(emp => emp.EmployeeNumber);
            e.HasIndex(emp => emp.NationalId);
        });

        builder.Entity<HrEmployeeContract>(e =>
        {
            e.HasOne(c => c.Employee).WithMany(emp => emp.Contracts).HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrEmployeeDocument>(e =>
        {
            e.HasOne(d => d.Employee).WithMany(emp => emp.Documents).HasForeignKey(d => d.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrFingerprintDevice>(e =>
        {
            e.HasOne(d => d.Branch).WithMany().HasForeignKey(d => d.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrFingerprintRecord>(e =>
        {
            e.HasOne(r => r.Employee).WithMany(emp => emp.FingerprintRecords).HasForeignKey(r => r.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(r => r.Device).WithMany().HasForeignKey(r => r.DeviceId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(r => new { r.EmployeeId, r.RecordDate });
        });

        builder.Entity<HrDailyAttendance>(e =>
        {
            e.HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.WorkShift).WithMany().HasForeignKey(a => a.WorkShiftId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(a => new { a.EmployeeId, a.AttendanceDate }).IsUnique();
        });

        builder.Entity<HrOvertimeRequest>(e =>
        {
            e.HasOne(o => o.Employee).WithMany().HasForeignKey(o => o.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrSalaryDetail>(e =>
        {
            e.HasOne(s => s.Employee).WithMany(emp => emp.SalaryDetails).HasForeignKey(s => s.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrSalaryAllowance>(e =>
        {
            e.HasOne(a => a.SalaryDetail).WithMany(s => s.Allowances).HasForeignKey(a => a.SalaryDetailId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.AllowanceType).WithMany().HasForeignKey(a => a.AllowanceTypeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrSalaryDeduction>(e =>
        {
            e.HasOne(d => d.SalaryDetail).WithMany(s => s.Deductions).HasForeignKey(d => d.SalaryDetailId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(d => d.DeductionType).WithMany().HasForeignKey(d => d.DeductionTypeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrMonthlyPayroll>(e =>
        {
            e.HasOne(p => p.Branch).WithMany().HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(p => new { p.SchoolId, p.BranchId, p.Month, p.Year }).IsUnique();
        });

        builder.Entity<HrPayrollItem>(e =>
        {
            e.HasOne(pi => pi.MonthlyPayroll).WithMany(p => p.PayrollItems).HasForeignKey(pi => pi.MonthlyPayrollId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(pi => pi.Employee).WithMany().HasForeignKey(pi => pi.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrSalaryAdvance>(e =>
        {
            e.HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrAdvanceDeductionLog>(e =>
        {
            e.HasOne(l => l.SalaryAdvance).WithMany(a => a.DeductionLogs).HasForeignKey(l => l.SalaryAdvanceId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrEmployeeLoan>(e =>
        {
            e.HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrLoanRepaymentLog>(e =>
        {
            e.HasOne(l => l.EmployeeLoan).WithMany(el => el.RepaymentLogs).HasForeignKey(l => l.EmployeeLoanId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrBonus>(e =>
        {
            e.HasOne(b => b.Employee).WithMany().HasForeignKey(b => b.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrPenalty>(e =>
        {
            e.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrPromotion>(e =>
        {
            e.HasOne(p => p.Employee).WithMany(emp => emp.Promotions).HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrCareerHistory>(e =>
        {
            e.HasOne(c => c.Employee).WithMany().HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrLeaveRequest>(e =>
        {
            e.HasOne(l => l.Employee).WithMany(emp => emp.LeaveRequests).HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrLeaveBalance>(e =>
        {
            e.HasOne(b => b.Employee).WithMany().HasForeignKey(b => b.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(b => b.LeaveType).WithMany().HasForeignKey(b => b.LeaveTypeId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(b => new { b.EmployeeId, b.LeaveTypeId, b.Year }).IsUnique();
        });

        builder.Entity<HrHoliday>(e =>
        {
            e.HasOne(h => h.Branch).WithMany().HasForeignKey(h => h.BranchId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrPerformanceReview>(e =>
        {
            e.HasOne(r => r.Employee).WithMany(emp => emp.PerformanceReviews).HasForeignKey(r => r.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(r => r.PerformanceCycle).WithMany(c => c.Reviews).HasForeignKey(r => r.PerformanceCycleId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrPerformanceScore>(e =>
        {
            e.HasOne(s => s.PerformanceReview).WithMany(r => r.Scores).HasForeignKey(s => s.PerformanceReviewId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(s => s.Criteria).WithMany().HasForeignKey(s => s.PerformanceCriteriaId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrKpi>(e =>
        {
            e.HasOne(k => k.Employee).WithMany().HasForeignKey(k => k.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(k => k.PerformanceCycle).WithMany().HasForeignKey(k => k.PerformanceCycleId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrTrainingRecord>(e =>
        {
            e.HasOne(t => t.Employee).WithMany(emp => emp.TrainingRecords).HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(t => t.TrainingProgram).WithMany(tp => tp.Participants).HasForeignKey(t => t.TrainingProgramId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrTrainingRequest>(e =>
        {
            e.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(t => t.AssignedTrainingProgram).WithMany().HasForeignKey(t => t.AssignedTrainingProgramId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrProfessionalCertificate>(e =>
        {
            e.HasOne(c => c.Employee).WithMany().HasForeignKey(c => c.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrDisciplinaryAction>(e =>
        {
            e.HasOne(d => d.Employee).WithMany(emp => emp.DisciplinaryActions).HasForeignKey(d => d.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(d => d.ViolationType).WithMany().HasForeignKey(d => d.ViolationTypeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrEndOfService>(e =>
        {
            e.HasOne(eos => eos.Employee).WithMany().HasForeignKey(eos => eos.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrClearanceItem>(e =>
        {
            e.HasOne(ci => ci.EndOfService).WithMany(eos => eos.ClearanceItems).HasForeignKey(ci => ci.EndOfServiceId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<HrEmployeeRequest>(e =>
        {
            e.HasOne(r => r.Employee).WithMany().HasForeignKey(r => r.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        });

        // Video Quiz & Notes
        builder.Entity<VideoQuizQuestion>(e =>
        {
            e.HasOne(q => q.CourseVideo).WithMany(cv => cv.QuizQuestions).HasForeignKey(q => q.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoQuizAnswer>(e =>
        {
            e.HasOne(a => a.VideoQuizQuestion).WithMany(q => q.Answers).HasForeignKey(a => a.VideoQuizQuestionId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<VideoNote>(e =>
        {
            e.HasOne(n => n.CourseVideo).WithMany(cv => cv.Notes).HasForeignKey(n => n.CourseVideoId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(n => n.Student).WithMany().HasForeignKey(n => n.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        // Decimal precision
        foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }
    }

    private void ApplyGlobalFilters<T>(ModelBuilder builder) where T : BaseEntity
    {
        // The filter uses the public CurrentSchoolId property so EF Core can
        // correctly parameterize it across the cached model. EF Core reads
        // the property from the CURRENT DbContext instance on every query.
        builder.Entity<T>().HasQueryFilter(e =>
            !e.IsDeleted &&
            (CurrentSchoolId == null || e.SchoolId == CurrentSchoolId));
    }

    private void ApplyTenantAndAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    if (entry.Entity.SchoolId == 0)
                    {
                        if (CurrentSchoolId.HasValue)
                            entry.Entity.SchoolId = CurrentSchoolId.Value;
                        else if (entry.Entity is not School)
                            throw new InvalidOperationException(
                                $"Cannot save {entry.Entity.GetType().Name}: SchoolId is 0 and no tenant context is available. " +
                                "Ensure the user has a SchoolId claim or set SchoolId explicitly.");
                    }
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            if (entry.State == EntityState.Added && entry.Entity.SchoolId == 0)
            {
                if (CurrentSchoolId.HasValue)
                    entry.Entity.SchoolId = CurrentSchoolId.Value;
                else
                    throw new InvalidOperationException(
                        "Cannot save ApplicationUser: SchoolId is 0 and no tenant context is available.");
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyTenantAndAuditFields();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantAndAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantAndAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTenantAndAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
