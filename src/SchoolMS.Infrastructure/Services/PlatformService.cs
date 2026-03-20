using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class PlatformService : IPlatformService
{
    private readonly SchoolDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlatformService(SchoolDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<PlatformDashboardDto> GetDashboardAsync()
    {
        var schools = await _context.Schools.IgnoreQueryFilters().Where(s => !s.IsDeleted).ToListAsync();
        var totalStudents = await _context.Students.IgnoreQueryFilters().CountAsync(s => !s.IsDeleted);
        var totalTeachers = await _context.Teachers.IgnoreQueryFilters().CountAsync(t => !t.IsDeleted);
        var activeSubscriptions = await _context.SchoolSubscriptions.IgnoreQueryFilters().CountAsync(s => s.IsActive && !s.IsDeleted);
        var expiringSoon = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .CountAsync(s => s.IsActive && !s.IsDeleted && s.ExpiryDate <= DateTime.UtcNow.AddDays(30));

        return new PlatformDashboardDto
        {
            TotalSchools = schools.Count,
            ActiveSchools = schools.Count(s => s.IsActive),
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            ActiveSubscriptions = activeSubscriptions,
            ExpiringSoon = expiringSoon,
            RecentSchools = schools.OrderByDescending(s => s.CreatedAt).Take(5).Select(s => new SchoolDto
            {
                Id = s.Id, Name = s.Name, Slug = s.Slug, IsActive = s.IsActive,
                Address = s.Address, ExpiryDate = s.ExpiryDate, CreatedAt = s.CreatedAt
            }).ToList()
        };
    }

    public async Task<List<SchoolDto>> GetAllSchoolsAsync()
    {
        var schools = await _context.Schools.IgnoreQueryFilters()
            .Where(s => !s.IsDeleted)
            .Include(s => s.Branches)
            .Include(s => s.SchoolSubscriptions).ThenInclude(ss => ss.SystemSubscriptionPlan)
            .ToListAsync();

        var schoolIds = schools.Select(s => s.Id).ToList();
        var studentCounts = await _context.Students.IgnoreQueryFilters()
            .Where(s => !s.IsDeleted && schoolIds.Contains(s.SchoolId))
            .GroupBy(s => s.SchoolId)
            .Select(g => new { SchoolId = g.Key, Count = g.Count() })
            .ToListAsync();
        var teacherCounts = await _context.Teachers.IgnoreQueryFilters()
            .Where(t => !t.IsDeleted && schoolIds.Contains(t.SchoolId))
            .GroupBy(t => t.SchoolId)
            .Select(g => new { SchoolId = g.Key, Count = g.Count() })
            .ToListAsync();

        return schools.Select(s => new SchoolDto
        {
            Id = s.Id, Name = s.Name, Logo = s.Logo, Address = s.Address, Slug = s.Slug,
            IsActive = s.IsActive, ExpiryDate = s.ExpiryDate, OnlinePlatformEnabled = s.OnlinePlatformEnabled,
            BranchCount = s.Branches.Count(b => !b.IsDeleted),
            StudentCount = studentCounts.FirstOrDefault(x => x.SchoolId == s.Id)?.Count ?? 0,
            TeacherCount = teacherCounts.FirstOrDefault(x => x.SchoolId == s.Id)?.Count ?? 0,
            CurrentPlan = s.SchoolSubscriptions.Where(ss => ss.IsActive && !ss.IsDeleted)
                .OrderByDescending(ss => ss.ActivatedAt).FirstOrDefault()?.SystemSubscriptionPlan?.PlanName,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<SchoolDto?> GetSchoolByIdAsync(int id)
    {
        var s = await _context.Schools.IgnoreQueryFilters()
            .Include(x => x.Branches)
            .Include(x => x.SchoolSubscriptions).ThenInclude(ss => ss.SystemSubscriptionPlan)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (s == null) return null;

        return new SchoolDto
        {
            Id = s.Id, Name = s.Name, Logo = s.Logo, Address = s.Address, Slug = s.Slug,
            IsActive = s.IsActive, ExpiryDate = s.ExpiryDate, OnlinePlatformEnabled = s.OnlinePlatformEnabled,
            BranchCount = s.Branches.Count(b => !b.IsDeleted),
            CurrentPlan = s.SchoolSubscriptions.Where(ss => ss.IsActive && !ss.IsDeleted)
                .OrderByDescending(ss => ss.ActivatedAt).FirstOrDefault()?.SystemSubscriptionPlan?.PlanName,
            CreatedAt = s.CreatedAt
        };
    }

    public async Task<SchoolDto> CreateSchoolAsync(SchoolCreateDto dto)
    {
        var slugExists = await _context.Schools.IgnoreQueryFilters().AnyAsync(s => s.Slug == dto.Slug && !s.IsDeleted);
        if (slugExists) throw new InvalidOperationException("School slug already exists.");

        var school = new School
        {
            Name = dto.Name, Address = dto.Address, Slug = dto.Slug,
            IsActive = dto.IsActive, ExpiryDate = dto.ExpiryDate,
            OnlinePlatformEnabled = dto.OnlinePlatformEnabled,
            SchoolId = 0, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        _context.Schools.Add(school);
        await _context.SaveChangesAsync();

        school.SchoolId = school.Id;
        await _context.SaveChangesAsync();

        var branch = new Branch
        {
            Name = "Main Branch", Address = dto.Address, IsActive = true,
            SchoolId = school.Id, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        var admin = new ApplicationUser
        {
            UserName = dto.AdminEmail, Email = dto.AdminEmail, FullName = dto.AdminFullName,
            SchoolId = school.Id, BranchId = branch.Id, UserType = UserType.Admin,
            EmailConfirmed = true, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        var result = await _userManager.CreateAsync(admin, dto.AdminPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"School created but admin user failed: {errors}");
        }
        await _userManager.AddToRoleAsync(admin, "SchoolAdmin");

        string[] pages = ["Dashboard", "Students", "Teachers", "Staff", "Subjects", "Divisions", "Grades", "ClassRooms",
            "TeacherAssignments", "Salaries", "Expenses", "Installments", "ExamSchedule", "WeeklySchedule",
            "StudentGrades", "Branches", "Leaves", "Attendance", "Notifications", "Users", "Promotion",
            "Homework", "Quizzes", "Carousel", "OnlinePlans", "OnlineSubscriptions", "PromoCodes",
            "Courses", "LiveStreams", "Chat",
            "Parents", "Events", "Announcements", "Behavior", "Health", "Complaints",
            "Visitors", "Library", "Transport", "Assets", "AuditLog"];
        string[] actions = ["View", "Add", "Edit", "Delete"];
        foreach (var page in pages)
            foreach (var action in actions)
                _context.Permissions.Add(new Permission { PageName = page, Action = action, SchoolId = school.Id, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform" });
        await _context.SaveChangesAsync();

        var allPerms = await _context.Permissions.IgnoreQueryFilters().Where(p => p.SchoolId == school.Id).ToListAsync();
        foreach (var perm in allPerms)
            _context.UserPermissions.Add(new UserPermission { UserId = admin.Id, PermissionId = perm.Id, IsGranted = true, SchoolId = school.Id, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform" });
        _context.UserBranches.Add(new UserBranch { UserId = admin.Id, BranchId = branch.Id, SchoolId = school.Id, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform" });
        await _context.SaveChangesAsync();

        if (dto.SubscriptionPlanId.HasValue)
        {
            var plan = await _context.SystemSubscriptionPlans.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == dto.SubscriptionPlanId.Value);
            if (plan != null)
            {
                _context.SchoolSubscriptions.Add(new SchoolSubscription
                {
                    SystemSubscriptionPlanId = plan.Id, ActivatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(plan.DurationMonths), IsActive = true,
                    SchoolId = school.Id, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
                });
                await _context.SaveChangesAsync();
            }
        }

        return new SchoolDto { Id = school.Id, Name = school.Name, Slug = school.Slug, IsActive = school.IsActive, CreatedAt = school.CreatedAt };
    }

    public async Task<SchoolDto> UpdateSchoolAsync(SchoolUpdateDto dto)
    {
        var school = await _context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == dto.Id && !s.IsDeleted)
            ?? throw new KeyNotFoundException("School not found.");
        school.Name = dto.Name; school.Address = dto.Address; school.Slug = dto.Slug;
        school.IsActive = dto.IsActive; school.ExpiryDate = dto.ExpiryDate;
        school.OnlinePlatformEnabled = dto.OnlinePlatformEnabled;
        school.UpdatedAt = DateTime.UtcNow; school.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
        return new SchoolDto { Id = school.Id, Name = school.Name, Slug = school.Slug, IsActive = school.IsActive, CreatedAt = school.CreatedAt };
    }

    public async Task ToggleSchoolActiveAsync(int id)
    {
        var school = await _context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new KeyNotFoundException("School not found.");
        school.IsActive = !school.IsActive;
        school.UpdatedAt = DateTime.UtcNow; school.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSchoolAsync(int id)
    {
        var school = await _context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new KeyNotFoundException("School not found.");
        school.IsDeleted = true; school.DeletedAt = DateTime.UtcNow; school.DeletedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    public async Task<List<SubscriptionPlanDto>> GetAllPlansAsync()
    {
        var plans = await _context.SystemSubscriptionPlans.IgnoreQueryFilters().Where(p => !p.IsDeleted).ToListAsync();
        var activeCounts = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Where(s => s.IsActive && !s.IsDeleted)
            .GroupBy(s => s.SystemSubscriptionPlanId)
            .Select(g => new { PlanId = g.Key, Count = g.Count() })
            .ToListAsync();

        return plans.Select(p => new SubscriptionPlanDto
        {
            Id = p.Id, PlanName = p.PlanName, Price = p.Price, MaxUsers = p.MaxUsers,
            MaxStudents = p.MaxStudents, DurationMonths = p.DurationMonths,
            IncludesHrModule = p.IncludesHrModule, IncludesCourses = p.IncludesCourses,
            IncludesLiveStream = p.IncludesLiveStream, StorageLimitGB = p.StorageLimitGB,
            ActiveSchoolCount = activeCounts.FirstOrDefault(x => x.PlanId == p.Id)?.Count ?? 0
        }).ToList();
    }

    public async Task<SubscriptionPlanDto> CreatePlanAsync(SubscriptionPlanDto dto)
    {
        var plan = new SystemSubscriptionPlan
        {
            PlanName = dto.PlanName, Price = dto.Price, MaxUsers = dto.MaxUsers,
            MaxStudents = dto.MaxStudents, DurationMonths = dto.DurationMonths,
            IncludesHrModule = dto.IncludesHrModule, IncludesCourses = dto.IncludesCourses,
            IncludesLiveStream = dto.IncludesLiveStream, StorageLimitGB = dto.StorageLimitGB,
            SchoolId = 1, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        _context.SystemSubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();
        dto.Id = plan.Id;
        return dto;
    }

    public async Task<SubscriptionPlanDto> UpdatePlanAsync(SubscriptionPlanDto dto)
    {
        var plan = await _context.SystemSubscriptionPlans.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == dto.Id && !p.IsDeleted)
            ?? throw new KeyNotFoundException();
        plan.PlanName = dto.PlanName; plan.Price = dto.Price; plan.MaxUsers = dto.MaxUsers;
        plan.MaxStudents = dto.MaxStudents; plan.DurationMonths = dto.DurationMonths;
        plan.IncludesHrModule = dto.IncludesHrModule; plan.IncludesCourses = dto.IncludesCourses;
        plan.IncludesLiveStream = dto.IncludesLiveStream; plan.StorageLimitGB = dto.StorageLimitGB;
        plan.UpdatedAt = DateTime.UtcNow; plan.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task DeletePlanAsync(int id)
    {
        var plan = await _context.SystemSubscriptionPlans.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException();
        plan.IsDeleted = true; plan.DeletedAt = DateTime.UtcNow; plan.DeletedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    public async Task<List<SchoolSubscriptionDto>> GetSchoolSubscriptionsAsync(int schoolId)
    {
        return await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Where(s => s.SchoolId == schoolId && !s.IsDeleted)
            .Include(s => s.SystemSubscriptionPlan)
            .OrderByDescending(s => s.ActivatedAt)
            .Select(s => new SchoolSubscriptionDto
            {
                Id = s.Id, SchoolId = s.SchoolId, SystemSubscriptionPlanId = s.SystemSubscriptionPlanId,
                PlanName = s.SystemSubscriptionPlan.PlanName,
                ActivatedAt = s.ActivatedAt, ExpiryDate = s.ExpiryDate, IsActive = s.IsActive,
                StorageLimitGB = s.SystemSubscriptionPlan.StorageLimitGB,
                ExtraStorageGB = s.ExtraStorageGB,
                ExtraStoragePrice = s.ExtraStoragePrice
            }).ToListAsync();
    }

    public async Task<SchoolSubscriptionDto> AssignSubscriptionAsync(SchoolSubscriptionDto dto)
    {
        var plan = await _context.SystemSubscriptionPlans.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == dto.SystemSubscriptionPlanId)
            ?? throw new KeyNotFoundException("Plan not found.");

        var existing = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Where(s => s.SchoolId == dto.SchoolId && s.IsActive && !s.IsDeleted).ToListAsync();
        foreach (var sub in existing) sub.IsActive = false;

        var subscription = new SchoolSubscription
        {
            SystemSubscriptionPlanId = dto.SystemSubscriptionPlanId,
            ActivatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(plan.DurationMonths),
            IsActive = true, SchoolId = dto.SchoolId,
            CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        _context.SchoolSubscriptions.Add(subscription);

        var school = await _context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == dto.SchoolId);
        if (school != null) school.ExpiryDate = subscription.ExpiryDate;

        await _context.SaveChangesAsync();
        dto.Id = subscription.Id;
        dto.ActivatedAt = subscription.ActivatedAt;
        dto.ExpiryDate = subscription.ExpiryDate;
        dto.IsActive = true;
        dto.PlanName = plan.PlanName;
        return dto;
    }

    public async Task CancelSubscriptionAsync(int subscriptionId)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted)
            ?? throw new KeyNotFoundException();
        sub.IsActive = false;
        sub.UpdatedAt = DateTime.UtcNow; sub.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    public async Task RenewSubscriptionAsync(int subscriptionId)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted)
            ?? throw new KeyNotFoundException();

        var months = sub.SystemSubscriptionPlan.DurationMonths;
        var baseDate = sub.ExpiryDate > DateTime.UtcNow ? sub.ExpiryDate : DateTime.UtcNow;
        sub.ExpiryDate = baseDate.AddMonths(months);
        sub.IsActive = true;
        sub.UpdatedAt = DateTime.UtcNow;
        sub.UpdatedBy = "Platform";

        var school = await _context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == sub.SchoolId);
        if (school != null) school.ExpiryDate = sub.ExpiryDate;

        await _context.SaveChangesAsync();
    }

    public async Task AddExtraStorageAsync(int subscriptionId, decimal extraGB, decimal pricePerGB)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted)
            ?? throw new KeyNotFoundException();

        sub.ExtraStorageGB += extraGB;
        sub.ExtraStoragePrice += extraGB * pricePerGB;
        sub.UpdatedAt = DateTime.UtcNow;
        sub.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    public async Task<List<StorageRequestDto>> GetPendingStorageRequestsAsync()
    {
        return await _context.StorageRequests.IgnoreQueryFilters()
            .Where(r => !r.IsProcessed && !r.IsDeleted)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.School)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.SystemSubscriptionPlan)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new StorageRequestDto
            {
                Id = r.Id,
                SchoolSubscriptionId = r.SchoolSubscriptionId,
                SchoolId = r.SchoolSubscription.SchoolId,
                SchoolName = r.SchoolSubscription.School.Name,
                PlanName = r.SchoolSubscription.SystemSubscriptionPlan.PlanName,
                RequestedGB = r.RequestedGB,
                PricePerGB = r.PricePerGB,
                TotalPrice = r.TotalPrice,
                Notes = r.Notes,
                IsApproved = r.IsApproved,
                IsProcessed = r.IsProcessed,
                ProcessedAt = r.ProcessedAt,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task ApproveStorageRequestAsync(int requestId, string approvedBy)
    {
        var request = await _context.StorageRequests.IgnoreQueryFilters()
            .Include(r => r.SchoolSubscription)
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted)
            ?? throw new KeyNotFoundException("Storage request not found.");

        request.IsApproved = true;
        request.IsProcessed = true;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedBy = approvedBy;

        request.SchoolSubscription.ExtraStorageGB += request.RequestedGB;
        request.SchoolSubscription.ExtraStoragePrice += request.TotalPrice;

        await _context.SaveChangesAsync();
    }

    public async Task RejectStorageRequestAsync(int requestId, string rejectedBy)
    {
        var request = await _context.StorageRequests.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted)
            ?? throw new KeyNotFoundException("Storage request not found.");

        request.IsApproved = false;
        request.IsProcessed = true;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedBy = rejectedBy;

        await _context.SaveChangesAsync();
    }
}
