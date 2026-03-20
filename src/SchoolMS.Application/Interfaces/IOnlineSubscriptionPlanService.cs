using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IOnlineSubscriptionPlanService
{
    Task<List<OnlineSubscriptionPlanDto>> GetAllAsync();
    Task<List<OnlineSubscriptionPlanDto>> GetBySchoolIdAsync(int schoolId, string? search = null);
    Task<OnlineSubscriptionPlanDto?> GetByIdAsync(int id);
    Task<OnlineSubscriptionPlanDto> CreateAsync(OnlineSubscriptionPlanDto dto);
    Task<OnlineSubscriptionPlanDto> UpdateAsync(OnlineSubscriptionPlanDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}
