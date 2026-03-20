namespace SchoolMS.Domain.Entities;

public class TeacherAssignment : BaseEntity
{
    public int TeacherId { get; set; }
    public int ClassRoomId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
    public int BranchId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
