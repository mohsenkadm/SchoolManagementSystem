using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrMonthlyPayroll : BaseEntity
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string? PayrollPeriod { get; set; }
    public int BranchId { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalBaseSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalBonuses { get; set; }
    public decimal TotalPenalties { get; set; }
    public decimal TotalOvertimeAmount { get; set; }
    public decimal TotalAbsenceDeductions { get; set; }
    public decimal TotalLateDeductions { get; set; }
    public decimal TotalAdvanceDeductions { get; set; }
    public decimal TotalLoanDeductions { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public PayrollStatus Status { get; set; }
    public string? PreparedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<HrPayrollItem> PayrollItems { get; set; } = new List<HrPayrollItem>();
}
