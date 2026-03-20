namespace SchoolMS.Domain.Entities;

public class HrPayrollItem : BaseEntity
{
    public int MonthlyPayrollId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? DepartmentName { get; set; }
    public string? JobTitleName { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal FoodAllowance { get; set; }
    public decimal PhoneAllowance { get; set; }
    public decimal PositionAllowance { get; set; }
    public decimal FamilyAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal TotalAllowances { get; set; }
    public string? AllowancesBreakdown { get; set; }

    public decimal SocialSecurityDeduction { get; set; }
    public decimal TaxDeduction { get; set; }
    public decimal InsuranceDeduction { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalFixedDeductions { get; set; }
    public string? DeductionsBreakdown { get; set; }

    public int WorkingDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int EarlyLeaveDays { get; set; }
    public int LeaveDays { get; set; }
    public int HolidayDays { get; set; }
    public decimal TotalLateMinutes { get; set; }
    public decimal TotalEarlyLeaveMinutes { get; set; }
    public decimal TotalOvertimeHours { get; set; }

    public decimal AbsenceDeduction { get; set; }
    public decimal LateDeduction { get; set; }
    public decimal EarlyLeaveDeduction { get; set; }
    public decimal OvertimeAmount { get; set; }

    public decimal BonusAmount { get; set; }
    public string? BonusReason { get; set; }
    public decimal PenaltyAmount { get; set; }
    public string? PenaltyReason { get; set; }

    public decimal AdvanceDeduction { get; set; }
    public decimal LoanDeduction { get; set; }

    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }

    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    public string? BankAccount { get; set; }
    public string? Notes { get; set; }

    public virtual HrMonthlyPayroll MonthlyPayroll { get; set; } = null!;
    public virtual HrEmployee Employee { get; set; } = null!;
}
