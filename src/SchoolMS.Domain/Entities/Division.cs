namespace SchoolMS.Domain.Entities;

public class Division : BaseEntity
{
    public string DivisionName { get; set; } = string.Empty;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
}
