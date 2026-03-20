namespace SchoolMS.Domain.Entities;

public class HrDepartment : BaseEntity
{
    public string DepartmentName { get; set; } = string.Empty;
    public string? DepartmentNameAr { get; set; }
    public string? DepartmentCode { get; set; }
    public string? Description { get; set; }
    public int? ParentDepartmentId { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public int BranchId { get; set; }
    public bool IsActive { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }

    public virtual HrDepartment? ParentDepartment { get; set; }
    public virtual ICollection<HrDepartment> SubDepartments { get; set; } = new List<HrDepartment>();
    public virtual ICollection<HrEmployee> Employees { get; set; } = new List<HrEmployee>();
    public virtual Branch Branch { get; set; } = null!;
}
