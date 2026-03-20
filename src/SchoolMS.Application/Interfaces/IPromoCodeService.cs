using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IPromoCodeService
{
    Task<List<PromoCodeDto>> GetAllAsync();
    Task<List<PromoCodeDto>> GetBySchoolIdAsync(int schoolId);
    Task<PromoCodeDto?> GetByIdAsync(int id);
    Task<PromoCodeDto?> GetByCodeAsync(string code);
    Task<PromoCodeDto> CreateAsync(PromoCodeDto dto);
    Task<PromoCodeDto> UpdateAsync(PromoCodeDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
    Task<(bool valid, string? error, decimal discountAmount)> ValidateAndCalculateDiscountAsync(string code, int studentId, decimal originalAmount);
    Task RecordUsageAsync(int promoCodeId, int studentId, int studentSubscriptionId);
}
