using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AssetService : IAssetService
{
    private readonly IRepository<AssetCategory> _catRepo;
    private readonly IRepository<Asset> _assetRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AssetService(IRepository<AssetCategory> catRepo, IRepository<Asset> assetRepo, IUnitOfWork unitOfWork)
    { _catRepo = catRepo; _assetRepo = assetRepo; _unitOfWork = unitOfWork; }

    public async Task<List<AssetCategoryDto>> GetAllCategoriesAsync()
    {
        var items = await _catRepo.Query().Include(c => c.Assets).ToListAsync();
        return items.Select(c => new AssetCategoryDto
        {
            Id = c.Id, CategoryName = c.CategoryName, Icon = c.Icon,
            AssetCount = c.Assets.Count(a => !a.IsDeleted)
        }).ToList();
    }

    public async Task<AssetCategoryDto> CreateCategoryAsync(AssetCategoryDto dto)
    {
        var entity = new AssetCategory { CategoryName = dto.CategoryName, Icon = dto.Icon };
        await _catRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var e = await _catRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _catRepo.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<AssetDto>> GetAllAssetsAsync()
    {
        var items = await _assetRepo.Query().Include(a => a.AssetCategory).Include(a => a.Branch).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<AssetDto?> GetAssetByIdAsync(int id)
    {
        var a = await _assetRepo.Query().Include(x => x.AssetCategory).Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        return a == null ? null : MapToDto(a);
    }

    public async Task<AssetDto> CreateAssetAsync(AssetDto dto)
    {
        var entity = new Asset
        {
            AssetName = dto.AssetName, AssetCategoryId = dto.AssetCategoryId, AssetCode = dto.AssetCode,
            SerialNumber = dto.SerialNumber, Description = dto.Description, PurchaseDate = dto.PurchaseDate,
            PurchasePrice = dto.PurchasePrice, CurrentValue = dto.CurrentValue, Condition = dto.Condition,
            Location = dto.Location, BranchId = dto.BranchId, Status = dto.Status, AssignedTo = dto.AssignedTo,
            SchoolId = dto.SchoolId
        };
        await _assetRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task<AssetDto> UpdateAssetAsync(AssetDto dto)
    {
        var entity = await _assetRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.AssetName = dto.AssetName; entity.AssetCategoryId = dto.AssetCategoryId; entity.AssetCode = dto.AssetCode;
        entity.SerialNumber = dto.SerialNumber; entity.Description = dto.Description; entity.PurchaseDate = dto.PurchaseDate;
        entity.PurchasePrice = dto.PurchasePrice; entity.CurrentValue = dto.CurrentValue; entity.Condition = dto.Condition;
        entity.Location = dto.Location; entity.BranchId = dto.BranchId; entity.Status = dto.Status; entity.AssignedTo = dto.AssignedTo;
        entity.SchoolId = dto.SchoolId;
        _assetRepo.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAssetAsync(int id)
    {
        var e = await _assetRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _assetRepo.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    private static AssetDto MapToDto(Asset a) => new()
    {
        Id = a.Id, AssetName = a.AssetName, AssetCategoryId = a.AssetCategoryId,
        CategoryName = a.AssetCategory?.CategoryName, AssetCode = a.AssetCode, SerialNumber = a.SerialNumber,
        Description = a.Description, PurchaseDate = a.PurchaseDate, PurchasePrice = a.PurchasePrice,
        CurrentValue = a.CurrentValue, Condition = a.Condition, Location = a.Location, BranchId = a.BranchId,
        BranchName = a.Branch?.Name, Status = a.Status, AssignedTo = a.AssignedTo,
        SchoolId = a.SchoolId
    };
}

