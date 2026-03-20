using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetAnalyticsAsync(int? schoolId = null, int? branchId = null);
    Task<byte[]> ExportAnalyticsToExcelAsync(int? schoolId = null, int? branchId = null);
}
