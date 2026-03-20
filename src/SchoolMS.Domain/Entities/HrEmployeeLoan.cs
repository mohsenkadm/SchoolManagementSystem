using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrEmployeeLoan : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? LoanType { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public decimal TotalRepayment { get; set; }
    public int RepaymentMonths { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public decimal PaidSoFar { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime FirstInstallmentDate { get; set; }
    public LoanStatus Status { get; set; }
    public string? GuarantorName { get; set; }
    public string? GuarantorPhone { get; set; }
    public string? ApprovedBy { get; set; }
    public string? AttachmentPath { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual ICollection<HrLoanRepaymentLog> RepaymentLogs { get; set; } = new List<HrLoanRepaymentLog>();
}

public class HrLoanRepaymentLog : BaseEntity
{
    public int EmployeeLoanId { get; set; }
    public int? PayrollItemId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAfter { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployeeLoan EmployeeLoan { get; set; } = null!;
}
