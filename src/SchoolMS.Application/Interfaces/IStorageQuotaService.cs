using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStorageQuotaService
{
    Task<StorageQuotaDto?> GetQuotaAsync(int schoolId);
    Task<(bool allowed, string? error)> CanUploadAsync(int schoolId, long fileSizeBytes);
    Task AddUsedStorageAsync(int schoolId, long fileSizeBytes);
    Task RemoveUsedStorageAsync(int schoolId, long fileSizeBytes);

    // Storage Plans
    Task<List<StoragePlanDto>> GetActiveStoragePlansAsync();
    Task<List<StoragePlanDto>> GetAllStoragePlansAsync();
    Task<StoragePlanDto> CreateStoragePlanAsync(StoragePlanDto dto);
    Task<StoragePlanDto> UpdateStoragePlanAsync(StoragePlanDto dto);
    Task DeleteStoragePlanAsync(int planId);

    // Storage Requests
    Task<StorageRequestDto> RequestExtraStorageAsync(int schoolId, int storagePlanId, string? notes);
    Task<List<StorageRequestDto>> GetSchoolRequestsAsync(int schoolId);
    Task<List<StorageRequestDto>> GetPendingRequestsAsync();
    Task<List<StorageRequestDto>> GetAllRequestsAsync();
    Task ApproveStorageRequestAsync(int requestId, string approvedBy);
    Task RejectStorageRequestAsync(int requestId, string rejectedBy);
}
