using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Interfaces;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class SubscriptionLimitService : ISubscriptionLimitService
{
    private readonly SchoolDbContext _context;

    public SubscriptionLimitService(SchoolDbContext context) => _context = context;

    public async Task<SubscriptionStatus?> GetStatusAsync(int schoolId)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync();

        if (sub?.SystemSubscriptionPlan == null) return null;

        var currentStudents = await _context.Students.IgnoreQueryFilters()
            .CountAsync(s => s.SchoolId == schoolId && !s.IsDeleted);
        var currentUsers = await _context.Users
            .CountAsync(u => u.SchoolId == schoolId && !u.IsDeleted);

        return new SubscriptionStatus
        {
            PlanName = sub.SystemSubscriptionPlan.PlanName,
            ExpiryDate = sub.ExpiryDate,
            MaxStudents = sub.SystemSubscriptionPlan.MaxStudents,
            CurrentStudents = currentStudents,
            MaxUsers = sub.SystemSubscriptionPlan.MaxUsers,
            CurrentUsers = currentUsers
        };
    }

    public async Task<(bool allowed, string? error)> CanAddStudentAsync(int schoolId)
    {
        var status = await GetStatusAsync(schoolId);
        if (status == null) return (true, null); // No plan = no limits
        if (status.IsExpired) return (false, "School subscription has expired.");
        if (status.CurrentStudents >= status.MaxStudents)
            return (false, $"Maximum students limit reached ({status.MaxStudents}). Current: {status.CurrentStudents}.");
        return (true, null);
    }

    public async Task<(bool allowed, string? error)> CanAddUserAsync(int schoolId)
    {
        var status = await GetStatusAsync(schoolId);
        if (status == null) return (true, null);
        if (status.IsExpired) return (false, "School subscription has expired.");
        if (status.CurrentUsers >= status.MaxUsers)
            return (false, $"Maximum users limit reached ({status.MaxUsers}). Current: {status.CurrentUsers}.");
        return (true, null);
    }
}
