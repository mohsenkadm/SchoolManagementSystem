namespace SchoolMS.Domain.Entities;

public class Grade : BaseEntity
{
    public string GradeName { get; set; } = string.Empty;
    public int DivisionId { get; set; }
    public int BranchId { get; set; }

    public virtual Division Division { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
}
