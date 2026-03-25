using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<HrEmployee> _staffRepo;
    private readonly IRepository<InstallmentPayment> _paymentRepo;
    private readonly IRepository<Expense> _expenseRepo;
    private readonly IRepository<Attendance> _attendanceRepo;
    private readonly IRepository<SchoolSubscription> _subscriptionRepo;

    public DashboardService(
        IRepository<Student> studentRepo,
        IRepository<Teacher> teacherRepo,
        IRepository<HrEmployee> staffRepo,
        IRepository<InstallmentPayment> paymentRepo,
        IRepository<Expense> expenseRepo,
        IRepository<Attendance> attendanceRepo,
        IRepository<SchoolSubscription> subscriptionRepo)
    {
        _studentRepo = studentRepo;
        _teacherRepo = teacherRepo;
        _staffRepo = staffRepo;
        _paymentRepo = paymentRepo;
        _expenseRepo = expenseRepo;
        _attendanceRepo = attendanceRepo;
        _subscriptionRepo = subscriptionRepo;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(int? branchId, int? schoolId = null)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var today = now.Date;

        var studentQuery = _studentRepo.Query();
        var teacherQuery = _teacherRepo.Query();
        var staffQuery = _staffRepo.Query();
        var paymentQuery = _paymentRepo.Query();
        var expenseQuery = _expenseRepo.Query();

        if (branchId.HasValue)
        {
            studentQuery = studentQuery.Where(s => s.BranchId == branchId);
            teacherQuery = teacherQuery.Where(t => t.BranchId == branchId);
            staffQuery = staffQuery.Where(s => s.BranchId == branchId);
            expenseQuery = expenseQuery.Where(e => e.BranchId == branchId);
        }

        var totalStudents = await studentQuery.CountAsync();
        var totalTeachers = await teacherQuery.CountAsync();
        var totalStaff = await staffQuery.CountAsync();

        var revenueThisMonth = await paymentQuery
            .Where(p => p.PaidDate.HasValue && p.PaidDate.Value >= startOfMonth)
            .SumAsync(p => p.Amount);

        var expensesThisMonth = await expenseQuery
            .Where(e => e.Date >= startOfMonth)
            .SumAsync(e => e.Amount);

        var overdueCount = await paymentQuery
            .Where(p => p.Status == PaymentStatus.Overdue || (p.Status == PaymentStatus.Pending && p.DueDate < today))
            .CountAsync();

        var todayAttendance = await _attendanceRepo.Query()
            .Where(a => a.AttendanceDate == today && a.Type == AttendanceType.CheckIn)
            .CountAsync();
        var totalPeople = totalStudents + totalTeachers + totalStaff;
        var attendanceRate = totalPeople > 0 ? (double)todayAttendance / totalPeople * 100 : 0;

        // Monthly financials for last 6 months
        var monthlyFinancials = new List<MonthlyFinancialData>();
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);
            var monthName = monthStart.ToString("MMM yyyy");

            var monthRevenue = await paymentQuery
                .Where(p => p.PaidDate.HasValue && p.PaidDate.Value >= monthStart && p.PaidDate.Value < monthEnd)
                .SumAsync(p => p.Amount);
            var monthExpenses = await expenseQuery
                .Where(e => e.Date >= monthStart && e.Date < monthEnd)
                .SumAsync(e => e.Amount);

            monthlyFinancials.Add(new MonthlyFinancialData
            {
                Month = monthName,
                Revenue = monthRevenue,
                Expenses = monthExpenses
            });
        }

        // Subscription expiry alert (within 10 days or already expired)
        SubscriptionExpiryAlertDto? subscriptionAlert = null;
        if (schoolId.HasValue)
        {
            var activeSub = await _subscriptionRepo.Query()
                .Where(s => s.SchoolId == schoolId.Value && s.IsActive)
                .Include(s => s.SystemSubscriptionPlan)
                .OrderByDescending(s => s.ExpiryDate)
                .FirstOrDefaultAsync();

            if (activeSub != null)
            {
                var daysRemaining = (int)(activeSub.ExpiryDate - now).TotalDays;
                if (daysRemaining <= 10)
                {
                    subscriptionAlert = new SubscriptionExpiryAlertDto
                    {
                        PlanName = activeSub.SystemSubscriptionPlan.PlanName,
                        ExpiryDate = activeSub.ExpiryDate,
                        DaysRemaining = Math.Max(0, daysRemaining),
                        IsExpired = daysRemaining < 0
                    };
                }
            }
        }

        return new DashboardDto
        {
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            TotalStaff = totalStaff,
            RevenueThisMonth = revenueThisMonth,
            ExpensesThisMonth = expensesThisMonth,
            AttendanceRateToday = Math.Round(attendanceRate, 1),
            OverdueInstallments = overdueCount,
            MonthlyFinancials = monthlyFinancials,
            SubscriptionExpiryAlert = subscriptionAlert
        };
    }
}
