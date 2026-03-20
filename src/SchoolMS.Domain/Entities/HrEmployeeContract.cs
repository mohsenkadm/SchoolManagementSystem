using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrEmployeeContract : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? ContractNumber { get; set; }
    public ContractType ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal AgreedSalary { get; set; }
    public string? Terms { get; set; }
    public string? AttachmentPath { get; set; }
    public ContractStatus Status { get; set; }
    public int? RenewedFromContractId { get; set; }
    public DateTime? RenewalDate { get; set; }
    public string? Notes { get; set; }
    public string? SignedBy { get; set; }
    public DateTime? SignedDate { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
