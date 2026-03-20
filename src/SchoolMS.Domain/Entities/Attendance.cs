using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class Attendance : BaseEntity
{
    public int PersonId { get; set; }
    public PersonType PersonType { get; set; }
    public string? BadgeCardNumber { get; set; }
    public DateTime AttendanceDate { get; set; }
    public TimeSpan Time { get; set; }
    public AttendanceType Type { get; set; }
    public int BranchId { get; set; }
    public bool IsAutoAbsent { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
