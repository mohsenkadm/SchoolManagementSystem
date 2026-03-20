using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class StudentPromotion : BaseEntity
{
    public int StudentId { get; set; }
    public int FromClassRoomId { get; set; }
    public int ToClassRoomId { get; set; }
    public int FromAcademicYearId { get; set; }
    public int ToAcademicYearId { get; set; }
    public PromotionStatus Status { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual ClassRoom FromClassRoom { get; set; } = null!;
    public virtual ClassRoom ToClassRoom { get; set; } = null!;
    public virtual AcademicYear FromAcademicYear { get; set; } = null!;
    public virtual AcademicYear ToAcademicYear { get; set; } = null!;
}
