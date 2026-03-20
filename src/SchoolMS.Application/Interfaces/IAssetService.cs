using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IAssetService
{
    Task<List<AssetCategoryDto>> GetAllCategoriesAsync();
    Task<AssetCategoryDto> CreateCategoryAsync(AssetCategoryDto dto);
    Task DeleteCategoryAsync(int id);
    Task<List<AssetDto>> GetAllAssetsAsync();
    Task<AssetDto?> GetAssetByIdAsync(int id);
    Task<AssetDto> CreateAssetAsync(AssetDto dto);
    Task<AssetDto> UpdateAssetAsync(AssetDto dto);
    Task DeleteAssetAsync(int id);
}

