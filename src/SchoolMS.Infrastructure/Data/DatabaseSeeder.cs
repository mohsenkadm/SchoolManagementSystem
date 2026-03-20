using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Infrastructure.Data;

public static class DatabaseSeeder
{
    // ── Permission page list (shared by every school) ──────────────────
    private static readonly string[] PermissionPages = [
        "Dashboard","Students","Teachers","Staff","Subjects","Divisions","Grades","ClassRooms",
        "TeacherAssignments","Salaries","Expenses","Installments","ExamSchedule","WeeklySchedule",
        "StudentGrades","Branches","Leaves","Attendance","Notifications","Users","Promotion",
        "Homework","Quizzes","Carousel","OnlinePlans","OnlineSubscriptions","PromoCodes",
        "Courses","LiveStreams","Chat",
        "Parents","Events","Announcements","Behavior","Health","Complaints",
        "Visitors","Library","Transport","Assets","AuditLog","Reports",
        "HrDashboard","HrDepartments","HrJobTitles","HrJobGrades","HrEmployees",
        "HrContracts","HrWorkShifts","HrFingerprint","HrAttendance","HrOvertime",
        "HrSalary","HrAdvances","HrLoans","HrBonuses","HrPenalties","HrPayroll",
        "HrPromotions","HrLeaves","HrLeaveTypes","HrHolidays",
        "HrPerformance","HrTraining","HrDisciplinary","HrEndOfService","HrSettings"
    ];
    private static readonly string[] PermissionActions = ["View", "Add", "Edit", "Delete"];

    // ───────────────────────────────────────────────────────────────────
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // ── 1. Roles ──────────────────────────────────────────────────
        string[] roles = ["SuperAdmin", "SchoolAdmin", "BranchAdmin", "Teacher", "DataEntry", "Student", "Staff"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        if (await context.Schools.IgnoreQueryFilters().AnyAsync())
            return; // already seeded

        var now = DateTime.UtcNow;

        // ── 2. Create first school so we have a valid SchoolId for plans ─
        var school1 = CreateSchoolRecord(context, "Al-Noor International School", "al-noor", "45 Knowledge Blvd, Riyadh", now);
        await context.SaveChangesAsync();
        school1.SchoolId = school1.Id;
        await context.SaveChangesAsync();

        // ── 3. System Subscription Plans (owned by school1 as platform host)
        var basicPlan = new SystemSubscriptionPlan { PlanName = "Basic", Price = 299m, MaxUsers = 10, MaxStudents = 100, DurationMonths = 12, SchoolId = school1.Id, CreatedAt = now, CreatedBy = "System" };
        var premiumPlan = new SystemSubscriptionPlan { PlanName = "Premium", Price = 999m, MaxUsers = 50, MaxStudents = 500, DurationMonths = 12, IncludesHrModule = true, SchoolId = school1.Id, CreatedAt = now, CreatedBy = "System" };
        context.SystemSubscriptionPlans.AddRange(basicPlan, premiumPlan);
        await context.SaveChangesAsync();

        // ── 4. Seed full data for school1 (already created) ───────────
        await SeedSchoolDataAsync(context, userManager, school1, new SchoolSeedInfo
        {
            Name = school1.Name, Slug = school1.Slug, Address = school1.Address!,
            AdminEmail = "admin@al-noor.edu", AdminFullName = "Khalid Al-Rashidi", AdminPassword = "Admin@1234",
            Teachers = [
                ("Ahmed Al-Mahmoud", "Mathematics", "+966501000001", "ahmed@al-noor.edu"),
                ("Sara Al-Fahad", "English", "+966501000002", "sara@al-noor.edu"),
                ("Omar Youssef", "Science", "+966501000003", "omar@al-noor.edu"),
                ("Layla Hassan", "Arabic", "+966501000004", "layla@al-noor.edu")
            ],
            StudentPrefix = "NOR",
            Plan = premiumPlan
        }, now);

        // ── 5. Create second school with all data ─────────────────────
        var school2 = await CreateSchoolWithDataAsync(context, userManager, new SchoolSeedInfo
        {
            Name = "Bright Future Academy", Slug = "bright-future", Address = "12 Innovation St, Jeddah",
            AdminEmail = "admin@bright-future.edu", AdminFullName = "Fatima Al-Otaibi", AdminPassword = "Admin@1234",
            Teachers = [
                ("Mohammed Al-Qahtani", "Mathematics", "+966502000001", "mohammed@bright-future.edu"),
                ("Nora Al-Shehri", "English", "+966502000002", "nora@bright-future.edu"),
                ("Hassan Al-Dosari", "Science", "+966502000003", "hassan@bright-future.edu")
            ],
            StudentPrefix = "BFA",
            Plan = basicPlan
        }, now);

        // ── 4. SuperAdmin (platform-level, no specific school) ────────
        // SuperAdmin is assigned to school1 in DB (FK required), but will
        // NOT get a SchoolId claim when logging in without a slug.
        var superAdmin = new ApplicationUser
        {
            UserName = "superadmin@platform.com",
            Email = "superadmin@platform.com",
            FullName = "Platform Super Admin",
            SchoolId = school1.Id,
            UserType = UserType.SuperAdmin,
            EmailConfirmed = true,
            CreatedAt = now,
            CreatedBy = "System"
        };
        var saResult = await userManager.CreateAsync(superAdmin, "Super@1234");
        if (saResult.Succeeded)
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

        await context.SaveChangesAsync();
    }

    // ══════════════════════════════════════════════════════════════════
    // Helper: create a School record (no related data yet)
    // ══════════════════════════════════════════════════════════════════
    private static School CreateSchoolRecord(SchoolDbContext context, string name, string slug, string address, DateTime now)
    {
        var school = new School
        {
            Name = name, Slug = slug, Address = address,
            IsActive = true, OnlinePlatformEnabled = true, IsHrModuleEnabled = true,
            ExpiryDate = now.AddYears(1), SchoolId = 0,
            HrWorkDayStart = new TimeSpan(8, 0, 0), HrWorkDayEnd = new TimeSpan(16, 0, 0),
            HrWorkingDaysPerMonth = 22, HrLateGracePeriodMinutes = 15,
            HrOvertimeRateMultiplier = 1.5m, HrMaxOvertimeHoursPerMonth = 40,
            CreatedAt = now, CreatedBy = "System"
        };
        context.Schools.Add(school);
        return school;
    }

    // ══════════════════════════════════════════════════════════════════
    // Helper: create school + seed all related data in one call
    // ══════════════════════════════════════════════════════════════════
    private static async Task<School> CreateSchoolWithDataAsync(
        SchoolDbContext context,
        UserManager<ApplicationUser> userManager,
        SchoolSeedInfo info,
        DateTime now)
    {
        var school = CreateSchoolRecord(context, info.Name, info.Slug, info.Address, now);
        await context.SaveChangesAsync();
        school.SchoolId = school.Id;
        await context.SaveChangesAsync();

        await SeedSchoolDataAsync(context, userManager, school, info, now);
        return school;
    }

    // ══════════════════════════════════════════════════════════════════
    // Helper: seed all related data for an already-created school
    // ══════════════════════════════════════════════════════════════════
    private static async Task SeedSchoolDataAsync(
        SchoolDbContext context,
        UserManager<ApplicationUser> userManager,
        School school,
        SchoolSeedInfo info,
        DateTime now)
    {
        int sid = school.Id;

        // ── Branches ──────────────────────────────────────────────────
        var mainBranch = new Branch { Name = "Main Branch", Address = info.Address, IsActive = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var secondBranch = new Branch { Name = "Second Branch", Address = info.Address + " - Annex", IsActive = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.Branches.AddRange(mainBranch, secondBranch);
        await context.SaveChangesAsync();

        // ── Academic Year ─────────────────────────────────────────────
        var ay = new AcademicYear { YearName = "2025-2026", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2026, 6, 30), IsCurrent = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.AcademicYears.Add(ay);
        await context.SaveChangesAsync();

        // ── Divisions ─────────────────────────────────────────────────
        var divA = new Division { DivisionName = "A", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var divB = new Division { DivisionName = "B", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.Divisions.AddRange(divA, divB);
        await context.SaveChangesAsync();

        // ── Grades ────────────────────────────────────────────────────
        var g1 = new Grade { GradeName = "Grade 1", DivisionId = divA.Id, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var g2 = new Grade { GradeName = "Grade 2", DivisionId = divA.Id, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var g3 = new Grade { GradeName = "Grade 3", DivisionId = divB.Id, BranchId = secondBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.Grades.AddRange(g1, g2, g3);
        await context.SaveChangesAsync();

        // ── Subjects ──────────────────────────────────────────────────
        var math = new Subject { SubjectName = "Mathematics", Description = "Math fundamentals", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var eng = new Subject { SubjectName = "English", Description = "English language", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var sci = new Subject { SubjectName = "Science", Description = "General science", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var arb = new Subject { SubjectName = "Arabic", Description = "Arabic language", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.Subjects.AddRange(math, eng, sci, arb);
        await context.SaveChangesAsync();
        var subjects = new[] { math, eng, sci, arb };

        // ── Exam Types ────────────────────────────────────────────────
        var midYear = new ExamType { TypeName = "Mid-Year", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var final = new ExamType { TypeName = "Final", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var quizType = new ExamType { TypeName = "Quiz", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.ExamTypes.AddRange(midYear, final, quizType);
        await context.SaveChangesAsync();

        // ── ClassRooms ────────────────────────────────────────────────
        var cr1 = new ClassRoom { GradeId = g1.Id, DivisionId = divA.Id, AcademicYearId = ay.Id, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var cr2 = new ClassRoom { GradeId = g2.Id, DivisionId = divA.Id, AcademicYearId = ay.Id, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var cr3 = new ClassRoom { GradeId = g3.Id, DivisionId = divB.Id, AcademicYearId = ay.Id, BranchId = secondBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.ClassRooms.AddRange(cr1, cr2, cr3);
        await context.SaveChangesAsync();
        var classRooms = new[] { cr1, cr2, cr3 };

        // ── Teachers ──────────────────────────────────────────────────
        var teachers = new List<Teacher>();
        for (int i = 0; i < info.Teachers.Length; i++)
        {
            var (name, spec, phone, email) = info.Teachers[i];
            var t = new Teacher
            {
                FullName = name, Specialization = spec, Phone = phone, Email = email,
                BadgeCardNumber = $"{info.StudentPrefix}-T{i + 1:D3}", Username = email.Split('@')[0], Password = "hashed",
                BranchId = i % 2 == 0 ? mainBranch.Id : secondBranch.Id,
                BaseSalary = 5000m + i * 500m, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
            };
            teachers.Add(t);
        }
        context.Teachers.AddRange(teachers);
        await context.SaveChangesAsync();

        // ── Staff ─────────────────────────────────────────────────────
        var staff1 = new Staff { FullName = "Receptionist " + info.Slug, Position = "Receptionist", Phone = "+966500000001", BadgeCardNumber = $"{info.StudentPrefix}-S001", Username = "reception." + info.Slug, Password = "hashed", BranchId = mainBranch.Id, BaseSalary = 3500m, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var staff2 = new Staff { FullName = "Accountant " + info.Slug, Position = "Accountant", Phone = "+966500000002", BadgeCardNumber = $"{info.StudentPrefix}-S002", Username = "account." + info.Slug, Password = "hashed", BranchId = mainBranch.Id, BaseSalary = 4500m, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.StaffMembers.AddRange(staff1, staff2);
        await context.SaveChangesAsync();

        // ── Teacher Assignments ───────────────────────────────────────
        for (int t = 0; t < teachers.Count; t++)
        {
            var cr = classRooms[t % classRooms.Length];
            var sub = subjects[t % subjects.Length];
            context.TeacherAssignments.Add(new TeacherAssignment
            {
                TeacherId = teachers[t].Id, ClassRoomId = cr.Id, SubjectId = sub.Id,
                AcademicYearId = ay.Id, BranchId = cr.BranchId, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
            });
        }
        await context.SaveChangesAsync();

        // ── Parents ───────────────────────────────────────────────────
        var parents = new List<Parent>();
        string[] fatherNames = ["Khalid Nasser", "Ibrahim Saeed", "Tariq Faisal", "Waleed Ahmad", "Youssef Hamad"];
        string[] motherNames = ["Amal Khalid", "Huda Ibrahim", "Reem Tariq", "Maha Waleed", "Noura Youssef"];
        for (int i = 0; i < 5; i++)
        {
            var p = new Parent
            {
                FatherName = fatherNames[i], FatherPhone = $"+96655{sid}{i + 1:D4}",
                MotherName = motherNames[i], MotherPhone = $"+96656{sid}{i + 1:D4}",
                Address = $"{100 + i} Family Rd, {info.Slug}",
                SchoolId = sid, CreatedAt = now, CreatedBy = "System"
            };
            parents.Add(p);
        }
        context.Parents.AddRange(parents);
        await context.SaveChangesAsync();

        // ── Students ──────────────────────────────────────────────────
        string[] boyNames = ["Faisal", "Abdullah", "Sultan", "Nawaf", "Turki", "Saud", "Rayan", "Ziad", "Hamza", "Anas"];
        string[] girlNames = ["Lama", "Reem", "Dana", "Nouf", "Haya", "Joud", "Lina", "Yara", "Aseel", "Shahad"];
        var students = new List<Student>();
        for (int i = 0; i < 15; i++)
        {
            bool isBoy = i % 2 == 0;
            var sName = isBoy ? boyNames[i % boyNames.Length] : girlNames[i % girlNames.Length];
            var s = new Student
            {
                FullName = $"{sName} {fatherNames[i % 5].Split(' ')[0]}", FullNameAr = $"طالب {i + 1}",
                DateOfBirth = new DateTime(2012, (i % 12) + 1, (i % 28) + 1),
                Gender = isBoy ? "Male" : "Female",
                Phone = $"+96659{sid}{i + 1:D4}", ParentPhone = $"+96655{sid}{(i % 5) + 1:D4}",
                ParentName = fatherNames[i % 5],
                BadgeCardNumber = $"{info.StudentPrefix}-{i + 1:D4}", Username = $"std{sid}_{i + 1}", Password = "hashed",
                BranchId = i < 10 ? mainBranch.Id : secondBranch.Id,
                ClassRoomId = classRooms[i % classRooms.Length].Id,
                AcademicYearId = ay.Id, ParentId = parents[i % parents.Count].Id,
                SchoolId = sid, CreatedAt = now, CreatedBy = "System"
            };
            students.Add(s);
        }
        context.Students.AddRange(students);
        await context.SaveChangesAsync();

        // ── Expense Types & Expenses ──────────────────────────────────
        var expMaint = new ExpenseType { TypeName = "Maintenance", Description = "Building maintenance", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var expSupp = new ExpenseType { TypeName = "Supplies", Description = "Office supplies", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var expUtil = new ExpenseType { TypeName = "Utilities", Description = "Electricity, water, internet", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.ExpenseTypes.AddRange(expMaint, expSupp, expUtil);
        await context.SaveChangesAsync();

        context.Expenses.AddRange(
            new Expense { ExpenseTypeId = expMaint.Id, Amount = 1500m, Date = now.AddDays(-20), Description = "AC repair", BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Expense { ExpenseTypeId = expSupp.Id, Amount = 800m, Date = now.AddDays(-10), Description = "Printer paper and toner", BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Expense { ExpenseTypeId = expUtil.Id, Amount = 2200m, Date = now.AddDays(-5), Description = "Monthly electricity bill", BranchId = secondBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Salary Setups ─────────────────────────────────────────────
        foreach (var t in teachers)
        {
            context.SalarySetups.Add(new SalarySetup { PersonId = t.Id, PersonType = PersonType.Teacher, BaseSalary = t.BaseSalary, Allowances = 500m, Deductions = 0m, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
        }
        context.SalarySetups.Add(new SalarySetup { PersonId = staff1.Id, PersonType = PersonType.Staff, BaseSalary = staff1.BaseSalary, Allowances = 300m, Deductions = 0m, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
        context.SalarySetups.Add(new SalarySetup { PersonId = staff2.Id, PersonType = PersonType.Staff, BaseSalary = staff2.BaseSalary, Allowances = 300m, Deductions = 0m, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
        await context.SaveChangesAsync();

        // ── Fee Installments & Payments ───────────────────────────────
        foreach (var s in students.Take(5))
        {
            var fee = new FeeInstallment { StudentId = s.Id, TotalAmount = 12000m, NumberOfPayments = 3, AcademicYearId = ay.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
            context.FeeInstallments.Add(fee);
            await context.SaveChangesAsync();
            for (int p = 1; p <= 3; p++)
            {
                context.InstallmentPayments.Add(new InstallmentPayment
                {
                    FeeInstallmentId = fee.Id, PaymentNumber = p, Amount = 4000m,
                    DueDate = now.AddMonths(p), Status = p == 1 ? PaymentStatus.Paid : PaymentStatus.Pending,
                    PaidDate = p == 1 ? now.AddDays(-5) : null,
                    SchoolId = sid, CreatedAt = now, CreatedBy = "System"
                });
            }
        }
        await context.SaveChangesAsync();

        // ── Exam Schedules ────────────────────────────────────────────
        foreach (var cr in classRooms)
        {
            for (int i = 0; i < Math.Min(subjects.Length, teachers.Count); i++)
            {
                context.ExamSchedules.Add(new ExamSchedule
                {
                    ExamTypeId = midYear.Id, ClassRoomId = cr.Id, SubjectId = subjects[i].Id,
                    TeacherId = teachers[i].Id, ExamDate = now.AddMonths(3).AddDays(i),
                    StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0),
                    AcademicYearId = ay.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
                });
            }
        }
        await context.SaveChangesAsync();

        // ── Weekly Schedule ───────────────────────────────────────────
        var days = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday };
        for (int d = 0; d < days.Length; d++)
        {
            for (int p = 0; p < 2; p++) // 2 periods per day
            {
                var tIdx = (d + p) % teachers.Count;
                var sIdx = (d + p) % subjects.Length;
                context.WeeklySchedules.Add(new WeeklySchedule
                {
                    ClassRoomId = cr1.Id, SubjectId = subjects[sIdx].Id, TeacherId = teachers[tIdx].Id,
                    DayOfWeek = days[d], StartTime = new TimeSpan(8 + p * 2, 0, 0), EndTime = new TimeSpan(8 + p * 2 + 1, 30, 0),
                    AcademicYearId = ay.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
                });
            }
        }
        await context.SaveChangesAsync();

        // ── Student Grades ────────────────────────────────────────────
        foreach (var s in students.Take(10))
        {
            for (int si = 0; si < subjects.Length; si++)
            {
                var mark = 60m + (s.Id + si * 7) % 40; // 60-99
                context.StudentGrades.Add(new StudentGrade
                {
                    StudentId = s.Id, SubjectId = subjects[si].Id, ExamTypeId = midYear.Id,
                    Mark = mark, MaxMark = 100, GradeLetter = mark >= 90 ? "A" : mark >= 80 ? "B" : mark >= 70 ? "C" : "D",
                    AcademicYearId = ay.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
                });
            }
        }
        await context.SaveChangesAsync();

        // ── Attendance ────────────────────────────────────────────────
        var today = now.Date;
        for (int d = 0; d < 5; d++)
        {
            var date = today.AddDays(-d);
            foreach (var s in students.Take(10))
            {
                context.Attendances.Add(new Attendance { PersonId = s.Id, PersonType = PersonType.Student, BadgeCardNumber = s.BadgeCardNumber, AttendanceDate = date, Time = new TimeSpan(7, 30 + (s.Id % 20), 0), Type = AttendanceType.CheckIn, BranchId = s.BranchId, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
            }
            foreach (var t in teachers)
            {
                context.Attendances.Add(new Attendance { PersonId = t.Id, PersonType = PersonType.Teacher, BadgeCardNumber = t.BadgeCardNumber, AttendanceDate = date, Time = new TimeSpan(7, 20, 0), Type = AttendanceType.CheckIn, BranchId = t.BranchId, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
            }
        }
        await context.SaveChangesAsync();

        // ── Leave Requests ────────────────────────────────────────────
        context.LeaveRequests.AddRange(
            new LeaveRequest { PersonId = teachers[0].Id, PersonType = PersonType.Teacher, StartDate = now.AddDays(10), EndDate = now.AddDays(12), Reason = "Family event", Status = LeaveStatus.Approved, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new LeaveRequest { PersonId = students[0].Id, PersonType = PersonType.Student, StartDate = now.AddDays(5), EndDate = now.AddDays(5), Reason = "Medical appointment", Status = LeaveStatus.Pending, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Homework ──────────────────────────────────────────────────
        for (int i = 0; i < Math.Min(teachers.Count, subjects.Length); i++)
        {
            context.Homeworks.Add(new Homework
            {
                TeacherId = teachers[i].Id, ClassRoomId = classRooms[i % classRooms.Length].Id, SubjectId = subjects[i].Id,
                Title = $"{subjects[i].SubjectName} Homework Week {i + 1}", Description = $"Complete exercises 1-10 from chapter {i + 1}",
                StartDate = now.AddDays(-3), DueDate = now.AddDays(4),
                AcademicYearId = ay.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System"
            });
        }
        await context.SaveChangesAsync();

        // ── Notifications ─────────────────────────────────────────────
        context.Notifications.AddRange(
            new Notification { Title = "Welcome!", Message = "Welcome to the new academic year at " + info.Name, Target = NotificationTarget.All, IsSent = true, SentAt = now, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Notification { Title = "Exam Schedule Published", Message = "Mid-year exam schedule has been published.", Target = NotificationTarget.All, IsSent = false, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Announcements ─────────────────────────────────────────────
        context.Announcements.AddRange(
            new Announcement { Title = "Welcome to " + info.Name, Content = "<p>We are delighted to welcome everyone to a new academic year.</p>", Priority = AnnouncementPriority.Important, Target = AnnouncementTarget.All, IsPinned = true, SendNotification = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Announcement { Title = "Uniform Policy Reminder", Content = "<p>All students must wear the school uniform starting next week.</p>", Priority = AnnouncementPriority.Normal, Target = AnnouncementTarget.Students, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── School Events ─────────────────────────────────────────────
        context.SchoolEvents.AddRange(
            new SchoolEvent { Title = "Annual Sports Day", Description = "Sports competitions for all grades", StartDate = now.AddMonths(1), EndDate = now.AddMonths(1).AddDays(1), EventCategory = EventType.Sports, Color = "#4cc9f0", IsAllDay = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new SchoolEvent { Title = "Parent-Teacher Meeting", Description = "Semester 1 PTM", StartDate = now.AddMonths(2), EndDate = now.AddMonths(2), EventCategory = EventType.Meeting, Color = "#f77f00", SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new SchoolEvent { Title = "Science Fair", Description = "Annual science projects exhibition", StartDate = now.AddMonths(3), EndDate = now.AddMonths(3), EventCategory = EventType.Activity, Color = "#06d6a0", SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Student Behaviors ─────────────────────────────────────────
        context.StudentBehaviors.AddRange(
            new StudentBehavior { StudentId = students[0].Id, Type = BehaviorType.Positive, Title = "Outstanding Participation", Description = "Actively participates in class discussions", Points = 10, RecordedBy = "Teacher", IncidentDate = now.AddDays(-3), SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new StudentBehavior { StudentId = students[1].Id, Type = BehaviorType.Warning, Title = "Late Submission", Description = "Submitted homework 2 days late", Points = -3, ActionTaken = "Verbal warning", RecordedBy = "Teacher", IncidentDate = now.AddDays(-1), NotifyParent = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Health Records ────────────────────────────────────────────
        context.HealthRecords.AddRange(
            new HealthRecord { StudentId = students[0].Id, RecordDate = now.AddDays(-10), RecordType = "Checkup", Title = "Annual Health Checkup", Description = "Routine annual checkup — all clear", DoctorName = "Dr. Saleh", SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new HealthRecord { StudentId = students[2].Id, RecordDate = now.AddDays(-5), RecordType = "Allergy", Title = "Peanut Allergy", Description = "Documented peanut allergy — epipen available", DoctorName = "Dr. Nadia", NotifyParent = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Complaints ────────────────────────────────────────────────
        context.Complaints.AddRange(
            new Complaint { TicketNumber = $"TK-{sid}-001", PersonId = students[0].Id, PersonType = PersonType.Student, PersonName = students[0].FullName, Subject = "Classroom Temperature", Description = "AC not working in classroom", Category = ComplaintCategory.Facility, Priority = ComplaintPriority.Medium, Status = ComplaintStatus.Open, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Complaint { TicketNumber = $"TK-{sid}-002", PersonId = teachers[0].Id, PersonType = PersonType.Teacher, PersonName = teachers[0].FullName, Subject = "Projector Issue", Description = "Projector in room 3 not working", Category = ComplaintCategory.Facility, Priority = ComplaintPriority.High, Status = ComplaintStatus.InProgress, AssignedTo = "Maintenance", SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Visitors ──────────────────────────────────────────────────
        context.Visitors.AddRange(
            new Visitor { VisitorName = "Saad Al-Mutairi", Phone = "+966551234567", Purpose = "Parent Meeting", VisitingPerson = teachers[0].FullName, CheckInTime = now.AddHours(-2), BadgeNumber = "V001", BranchId = mainBranch.Id, Status = VisitorStatus.CheckedIn, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Visitor { VisitorName = "Nasser Al-Otaibi", Phone = "+966559876543", Purpose = "Delivery", VisitingDepartment = "Admin", CheckInTime = now.AddHours(-4), CheckOutTime = now.AddHours(-3), BadgeNumber = "V002", BranchId = mainBranch.Id, Status = VisitorStatus.CheckedOut, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Library Books ─────────────────────────────────────────────
        context.LibraryBooks.AddRange(
            new LibraryBook { Title = "Mathematics Grade 1", Author = "Textbook Committee", Category = "Textbook", TotalCopies = 50, AvailableCopies = 48, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new LibraryBook { Title = "Arabic Literature", Author = "Prof. Hassan", Category = "Literature", TotalCopies = 30, AvailableCopies = 30, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new LibraryBook { Title = "Science Encyclopedia", Author = "Various Authors", Category = "Reference", TotalCopies = 10, AvailableCopies = 9, BranchId = secondBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new LibraryBook { Title = "English Grammar", Author = "Oxford Press", Category = "Textbook", TotalCopies = 40, AvailableCopies = 40, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Transport Routes ──────────────────────────────────────────
        var route1 = new TransportRoute { RouteName = "Route A - Downtown", DriverName = "Ali Mohammed", DriverPhone = "+966551110001", BusNumber = $"BUS-{sid}-01", Capacity = 40, MonthlyFee = 200m, BranchId = mainBranch.Id, IsActive = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var route2 = new TransportRoute { RouteName = "Route B - Suburbs", DriverName = "Omar Hassan", DriverPhone = "+966551110002", BusNumber = $"BUS-{sid}-02", Capacity = 35, MonthlyFee = 250m, BranchId = mainBranch.Id, IsActive = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.TransportRoutes.AddRange(route1, route2);
        await context.SaveChangesAsync();

        context.TransportStops.AddRange(
            new TransportStop { TransportRouteId = route1.Id, StopName = "Central Square", StopOrder = 1, PickupTime = new TimeSpan(6, 30, 0), DropoffTime = new TimeSpan(14, 0, 0), SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new TransportStop { TransportRouteId = route1.Id, StopName = "Market Street", StopOrder = 2, PickupTime = new TimeSpan(6, 45, 0), DropoffTime = new TimeSpan(14, 15, 0), SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new TransportStop { TransportRouteId = route2.Id, StopName = "Green Park", StopOrder = 1, PickupTime = new TimeSpan(6, 30, 0), DropoffTime = new TimeSpan(14, 0, 0), SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Asset Categories & Assets ─────────────────────────────────
        var catFurn = new AssetCategory { CategoryName = "Furniture", Icon = "fas fa-chair", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var catElec = new AssetCategory { CategoryName = "Electronics", Icon = "fas fa-laptop", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        var catSport = new AssetCategory { CategoryName = "Sports Equipment", Icon = "fas fa-football-ball", SchoolId = sid, CreatedAt = now, CreatedBy = "System" };
        context.AssetCategories.AddRange(catFurn, catElec, catSport);
        await context.SaveChangesAsync();

        context.Assets.AddRange(
            new Asset { AssetName = "Student Desk", AssetCategoryId = catFurn.Id, AssetCode = $"A-{sid}-001", PurchaseDate = now.AddYears(-1), PurchasePrice = 150m, CurrentValue = 120m, Condition = "Good", Location = "Room 101", BranchId = mainBranch.Id, Status = AssetStatus.InUse, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Asset { AssetName = "Projector", AssetCategoryId = catElec.Id, AssetCode = $"A-{sid}-002", PurchaseDate = now.AddMonths(-6), PurchasePrice = 2500m, CurrentValue = 2200m, Condition = "New", Location = "Room 201", BranchId = mainBranch.Id, Status = AssetStatus.InUse, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
            new Asset { AssetName = "Football Set", AssetCategoryId = catSport.Id, AssetCode = $"A-{sid}-003", PurchaseDate = now.AddMonths(-3), PurchasePrice = 300m, CurrentValue = 280m, Condition = "Good", Location = "Sports Room", BranchId = secondBranch.Id, Status = AssetStatus.Available, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // ── Permissions ───────────────────────────────────────────────
        foreach (var page in PermissionPages)
            foreach (var action in PermissionActions)
                context.Permissions.Add(new Permission { PageName = page, Action = action, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });
        await context.SaveChangesAsync();

        // ── Subscription ──────────────────────────────────────────────
        context.SchoolSubscriptions.Add(new SchoolSubscription
        {
            SystemSubscriptionPlanId = info.Plan.Id, ActivatedAt = now,
            ExpiryDate = now.AddMonths(info.Plan.DurationMonths), IsActive = true,
            SchoolId = sid, CreatedAt = now, CreatedBy = "System"
        });
        await context.SaveChangesAsync();

        // ── School Admin User ─────────────────────────────────────────
        var admin = new ApplicationUser
        {
            UserName = info.AdminEmail, Email = info.AdminEmail, FullName = info.AdminFullName,
            SchoolId = sid, BranchId = mainBranch.Id, UserType = UserType.Admin,
            EmailConfirmed = true, CreatedAt = now, CreatedBy = "System"
        };
        var adminResult = await userManager.CreateAsync(admin, info.AdminPassword);
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "SchoolAdmin");

            // Grant ALL permissions for this school
            var schoolPerms = await context.Permissions.IgnoreQueryFilters()
                .Where(p => p.SchoolId == sid && !p.IsDeleted).ToListAsync();
            foreach (var perm in schoolPerms)
                context.UserPermissions.Add(new UserPermission { UserId = admin.Id, PermissionId = perm.Id, IsGranted = true, SchoolId = sid, CreatedAt = now, CreatedBy = "System" });

            // Assign to all branches
            context.UserBranches.AddRange(
                new UserBranch { UserId = admin.Id, BranchId = mainBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" },
                new UserBranch { UserId = admin.Id, BranchId = secondBranch.Id, SchoolId = sid, CreatedAt = now, CreatedBy = "System" }
            );
            await context.SaveChangesAsync();
        }

        }

        // ── DTO for school seed configuration ─────────────────────────────
    private class SchoolSeedInfo
    {
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
        public string Address { get; init; } = default!;
        public string AdminEmail { get; init; } = default!;
        public string AdminFullName { get; init; } = default!;
        public string AdminPassword { get; init; } = default!;
        public (string Name, string Spec, string Phone, string Email)[] Teachers { get; init; } = [];
        public string StudentPrefix { get; init; } = default!;
        public SystemSubscriptionPlan Plan { get; init; } = default!;
    }
}
