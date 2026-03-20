using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(int? branchId);
}

