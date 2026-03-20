using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IAuditLogService
{
    Task<DataTableResponse<AuditLogDto>> GetDataTableAsync(DataTableRequest request);
    Task LogAsync(string userId, string userName, string action, string? entityName, int? entityId, string? oldValues, string? newValues, string? ipAddress, string? pageName);
}

