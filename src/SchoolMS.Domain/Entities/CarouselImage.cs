namespace SchoolMS.Domain.Entities;

public class CarouselImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int? BranchId { get; set; }

    public virtual Branch? Branch { get; set; }
}
