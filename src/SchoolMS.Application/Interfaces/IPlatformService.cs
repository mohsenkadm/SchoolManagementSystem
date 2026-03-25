using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IPlatformService
{
    Task<PlatformDashboardDto> GetDashboardAsync();
    Task<List<SchoolDto>> GetAllSchoolsAsync();
    Task<SchoolDto?> GetSchoolByIdAsync(int id);
    Task<SchoolDto> CreateSchoolAsync(SchoolCreateDto dto);
    Task<SchoolDto> UpdateSchoolAsync(SchoolUpdateDto dto);
    Task ToggleSchoolActiveAsync(int id);
    Task DeleteSchoolAsync(int id);

    Task<List<SubscriptionPlanDto>> GetAllPlansAsync();
    Task<SubscriptionPlanDto> CreatePlanAsync(SubscriptionPlanDto dto);
    Task<SubscriptionPlanDto> UpdatePlanAsync(SubscriptionPlanDto dto);
    Task DeletePlanAsync(int id);

    Task<List<SchoolSubscriptionDto>> GetSchoolSubscriptionsAsync(int schoolId);
    Task<SchoolSubscriptionDto> AssignSubscriptionAsync(SchoolSubscriptionDto dto);
    Task CancelSubscriptionAsync(int subscriptionId);
    Task RenewSubscriptionAsync(int subscriptionId);
    Task AddExtraStorageAsync(int subscriptionId, decimal extraGB, decimal pricePerGB);

    // Storage request management
    Task<List<StorageRequestDto>> GetPendingStorageRequestsAsync();
    Task<List<StorageRequestDto>> GetAllStorageRequestsAsync();
    Task ApproveStorageRequestAsync(int requestId, string approvedBy);
    Task RejectStorageRequestAsync(int requestId, string rejectedBy);
}
