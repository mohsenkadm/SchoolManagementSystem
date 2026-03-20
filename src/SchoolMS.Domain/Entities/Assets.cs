using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class AssetCategory : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryNameAr { get; set; }
    public string? Icon { get; set; }

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}

public class Asset : BaseEntity
{
    public string AssetName { get; set; } = string.Empty;
    public string? AssetNameAr { get; set; }
    public int AssetCategoryId { get; set; }
    public string? AssetCode { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentValue { get; set; }
    public string? Condition { get; set; }
    public string? Location { get; set; }
    public int BranchId { get; set; }
    public AssetStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public string? Image { get; set; }

    public virtual AssetCategory AssetCategory { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
