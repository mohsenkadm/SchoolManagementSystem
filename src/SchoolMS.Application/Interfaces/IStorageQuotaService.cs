using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStorageQuotaService
{
    Task<StorageQuotaDto?> GetQuotaAsync(int schoolId);
    Task<(bool allowed, string? error)> CanUploadAsync(int schoolId, long fileSizeBytes);
    Task AddUsedStorageAsync(int schoolId, long fileSizeBytes);
    Task RemoveUsedStorageAsync(int schoolId, long fileSizeBytes);
    Task<StorageRequestDto> RequestExtraStorageAsync(int schoolId, decimal requestedGB, string? notes);
    Task<List<StorageRequestDto>> GetPendingRequestsAsync();
    Task ApproveStorageRequestAsync(int requestId, string approvedBy);
    Task RejectStorageRequestAsync(int requestId, string rejectedBy);
}
