namespace SchoolMS.Domain.Entities;

public class HrJobTitle : BaseEntity
{
    public string TitleName { get; set; } = string.Empty;
    public string? TitleNameAr { get; set; }
    public string? TitleCode { get; set; }
    public string? Description { get; set; }
    public int? DepartmentId { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool IsActive { get; set; }

    public virtual HrDepartment? Department { get; set; }
}
