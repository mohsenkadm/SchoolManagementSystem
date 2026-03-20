using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class PromoCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxUsage { get; set; }
    public int CurrentUsage { get; set; }
    public bool IsUnlimited { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<PromoCodeUsage> Usages { get; set; } = new List<PromoCodeUsage>();
}
