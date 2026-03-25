using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrSalaryService : IHrSalaryService
{
    private readonly IRepository<HrSalaryDetail> _salaryRepo;
    private readonly IRepository<HrAllowanceType> _allowanceTypeRepo;
    private readonly IRepository<HrDeductionType> _deductionTypeRepo;
    private readonly IRepository<HrSalaryAllowance> _allowanceRepo;
    private readonly IRepository<HrSalaryDeduction> _deductionRepo;
    private readonly IRepository<HrMonthlyPayroll> _payrollRepo;
    private readonly IRepository<HrPayrollItem> _payrollItemRepo;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IRepository<HrSalaryAdvance> _advanceRepo;
    private readonly IRepository<HrEmployeeLoan> _loanRepo;
    private readonly IRepository<HrBonus> _bonusRepo;
    private readonly IRepository<HrPenalty> _penaltyRepo;
    private readonly IRepository<HrDailyAttendance> _attendanceRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrSalaryService(
        IRepository<HrSalaryDetail> salaryRepo, IRepository<HrAllowanceType> allowanceTypeRepo,
        IRepository<HrDeductionType> deductionTypeRepo, IRepository<HrSalaryAllowance> allowanceRepo,
        IRepository<HrSalaryDeduction> deductionRepo, IRepository<HrMonthlyPayroll> payrollRepo,
        IRepository<HrPayrollItem> payrollItemRepo, IRepository<HrEmployee> employeeRepo,
        IRepository<HrSalaryAdvance> advanceRepo, IRepository<HrEmployeeLoan> loanRepo,
        IRepository<HrBonus> bonusRepo, IRepository<HrPenalty> penaltyRepo,
        IRepository<HrDailyAttendance> attendanceRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _salaryRepo = salaryRepo;
        _allowanceTypeRepo = allowanceTypeRepo;
        _deductionTypeRepo = deductionTypeRepo;
        _allowanceRepo = allowanceRepo;
        _deductionRepo = deductionRepo;
        _payrollRepo = payrollRepo;
        _payrollItemRepo = payrollItemRepo;
        _employeeRepo = employeeRepo;
        _advanceRepo = advanceRepo;
        _loanRepo = loanRepo;
        _bonusRepo = bonusRepo;
        _penaltyRepo = penaltyRepo;
        _attendanceRepo = attendanceRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HrSalaryDetailDto?> GetCurrentSalaryAsync(int employeeId)
    {
        var salary = await _salaryRepo.Query()
            .Include(s => s.Allowances).ThenInclude(a => a.AllowanceType)
            .Include(s => s.Deductions).ThenInclude(d => d.DeductionType)
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.IsCurrent);
        if (salary == null) return null;

        return MapSalaryDetail(salary);
    }

    public async Task<List<HrSalaryDetailDto>> GetSalaryHistoryAsync(int employeeId)
    {
        var items = await _salaryRepo.Query()
            .Include(s => s.Allowances).ThenInclude(a => a.AllowanceType)
            .Include(s => s.Deductions).ThenInclude(d => d.DeductionType)
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveDate)
            .ToListAsync();
        return items.Select(MapSalaryDetail).ToList();
    }

    public async Task<HrSalaryDetailDto> CreateSalarySetupAsync(HrSalaryDetailDto dto)
    {
        // Deactivate current salary
        var current = await _salaryRepo.Query()
            .FirstOrDefaultAsync(s => s.EmployeeId == dto.EmployeeId && s.IsCurrent);
        if (current != null)
        {
            current.IsCurrent = false;
            current.EndDate = dto.EffectiveDate.AddDays(-1);
            _salaryRepo.Update(current);
        }

        var entity = new HrSalaryDetail
        {
            EmployeeId = dto.EmployeeId,
            BaseSalary = dto.BaseSalary,
            Currency = dto.Currency,
            SalaryType = dto.SalaryType,
            EffectiveDate = dto.EffectiveDate,
            IsCurrent = true,
            Notes = dto.Notes
        };
        await _salaryRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrSalaryDetailDto>(entity);
    }

    public async Task<HrSalaryDetailDto> UpdateSalarySetupAsync(HrSalaryDetailDto dto)
    {
        var entity = await _salaryRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Salary setup with ID {dto.Id} not found.");
        entity.BaseSalary = dto.BaseSalary;
        entity.Currency = dto.Currency;
        entity.SalaryType = dto.SalaryType;
        entity.Notes = dto.Notes;
        _salaryRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrSalaryDetailDto>(entity);
    }

    // Allowance Types
    public async Task<List<HrAllowanceTypeDto>> GetAllowanceTypesAsync()
        => _mapper.Map<List<HrAllowanceTypeDto>>(await _allowanceTypeRepo.GetAllAsync());

    public async Task<HrAllowanceTypeDto> CreateAllowanceTypeAsync(HrAllowanceTypeDto dto)
    {
        var entity = _mapper.Map<HrAllowanceType>(dto);
        entity.Id = 0;
        await _allowanceTypeRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrAllowanceTypeDto>(entity);
    }

    public async Task<HrAllowanceTypeDto> UpdateAllowanceTypeAsync(HrAllowanceTypeDto dto)
    {
        var entity = await _allowanceTypeRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"AllowanceType with ID {dto.Id} not found.");
        entity.AllowanceName = dto.AllowanceName;
        entity.AllowanceNameAr = dto.AllowanceNameAr;
        entity.AllowanceCode = dto.AllowanceCode;
        entity.CalculationType = dto.CalculationType;
        entity.DefaultValue = dto.DefaultValue;
        entity.IsTaxable = dto.IsTaxable;
        entity.IsRecurring = dto.IsRecurring;
        entity.IsActive = dto.IsActive;
        entity.Description = dto.Description;
        _allowanceTypeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrAllowanceTypeDto>(entity);
    }

    public async Task DeleteAllowanceTypeAsync(int id)
    {
        var entity = await _allowanceTypeRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"AllowanceType with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _allowanceTypeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // Deduction Types
    public async Task<List<HrDeductionTypeDto>> GetDeductionTypesAsync()
        => _mapper.Map<List<HrDeductionTypeDto>>(await _deductionTypeRepo.GetAllAsync());

    public async Task<HrDeductionTypeDto> CreateDeductionTypeAsync(HrDeductionTypeDto dto)
    {
        var entity = _mapper.Map<HrDeductionType>(dto);
        entity.Id = 0;
        await _deductionTypeRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDeductionTypeDto>(entity);
    }

    public async Task<HrDeductionTypeDto> UpdateDeductionTypeAsync(HrDeductionTypeDto dto)
    {
        var entity = await _deductionTypeRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"DeductionType with ID {dto.Id} not found.");
        entity.DeductionName = dto.DeductionName;
        entity.DeductionNameAr = dto.DeductionNameAr;
        entity.DeductionCode = dto.DeductionCode;
        entity.CalculationType = dto.CalculationType;
        entity.DefaultValue = dto.DefaultValue;
        entity.IsRecurring = dto.IsRecurring;
        entity.IsMandatory = dto.IsMandatory;
        entity.IsActive = dto.IsActive;
        entity.Description = dto.Description;
        _deductionTypeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDeductionTypeDto>(entity);
    }

    public async Task DeleteDeductionTypeAsync(int id)
    {
        var entity = await _deductionTypeRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"DeductionType with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _deductionTypeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // Payroll
    public async Task<HrMonthlyPayrollDto> GeneratePayrollAsync(int month, int year, int branchId)
    {
        var existing = await _payrollRepo.Query()
            .FirstOrDefaultAsync(p => p.Month == month && p.Year == year && p.BranchId == branchId);
        if (existing != null)
            throw new InvalidOperationException("Payroll already exists for this period and branch.");

        var employees = await _employeeRepo.Query()
            .Include(e => e.Department).Include(e => e.JobTitle)
            .Where(e => e.BranchId == branchId && e.Status == HrEmployeeStatus.Active)
            .ToListAsync();

        var payroll = new HrMonthlyPayroll
        {
            Month = month,
            Year = year,
            PayrollPeriod = $"{year}-{month:D2}",
            BranchId = branchId,
            TotalEmployees = employees.Count,
            Status = PayrollStatus.Draft
        };
        await _payrollRepo.AddAsync(payroll);
        await _unitOfWork.SaveChangesAsync();

        foreach (var emp in employees)
        {
            var salary = await _salaryRepo.Query()
                .Include(s => s.Allowances).Include(s => s.Deductions)
                .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id && s.IsCurrent);

            var baseSalary = salary?.BaseSalary ?? 0;
            var totalAllowances = salary?.Allowances.Where(a => a.IsActive).Sum(a => a.CalculatedAmount) ?? 0;
            var totalDeductions = salary?.Deductions.Where(d => d.IsActive).Sum(d => d.CalculatedAmount) ?? 0;

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var attendance = await _attendanceRepo.Query()
                .Where(a => a.EmployeeId == emp.Id && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
                .ToListAsync();

            var bonuses = await _bonusRepo.Query()
                .Where(b => b.EmployeeId == emp.Id && b.Month == month && b.Year == year && b.Status == BonusStatus.Approved && b.IncludeInPayroll)
                .ToListAsync();

            var penalties = await _penaltyRepo.Query()
                .Where(p => p.EmployeeId == emp.Id && p.Month == month && p.Year == year && p.Status == PenaltyStatus.Approved && p.IncludeInPayroll)
                .ToListAsync();

            var advances = await _advanceRepo.Query()
                .Where(a => a.EmployeeId == emp.Id && a.Status == AdvanceStatus.Approved && a.RemainingAmount > 0)
                .ToListAsync();

            var loans = await _loanRepo.Query()
                .Where(l => l.EmployeeId == emp.Id && l.Status == LoanStatus.Active && l.RemainingBalance > 0)
                .ToListAsync();

            var presentDays = attendance.Count(a => a.Status == DailyAttendanceStatus.Present || a.Status == DailyAttendanceStatus.Late || a.Status == DailyAttendanceStatus.EarlyLeave || a.Status == DailyAttendanceStatus.LateAndEarlyLeave);
            var absentDays = attendance.Count(a => a.IsAbsent);
            var lateDays = attendance.Count(a => a.IsLate);
            var bonusAmount = bonuses.Sum(b => b.Amount);
            var penaltyAmount = penalties.Sum(p => p.Amount);
            var absenceDeduction = attendance.Sum(a => a.AbsenceDeductionAmount ?? 0);
            var lateDeduction = attendance.Sum(a => a.LateDeductionAmount ?? 0);
            var overtimeAmount = attendance.Sum(a => a.OvertimeAmount ?? 0);
            var advanceDeduction = advances.Sum(a => Math.Min(a.MonthlyDeduction, a.RemainingAmount));
            var loanDeduction = loans.Sum(l => Math.Min(l.MonthlyInstallment, l.RemainingBalance));

            var grossSalary = baseSalary + totalAllowances + overtimeAmount + bonusAmount;
            var totalDeductionsAll = totalDeductions + absenceDeduction + lateDeduction + penaltyAmount + advanceDeduction + loanDeduction;
            var netSalary = grossSalary - totalDeductionsAll;

            var item = new HrPayrollItem
            {
                MonthlyPayrollId = payroll.Id,
                EmployeeId = emp.Id,
                EmployeeName = emp.FullName,
                EmployeeNumber = emp.EmployeeNumber,
                DepartmentName = emp.Department?.DepartmentName,
                JobTitleName = emp.JobTitle?.TitleName,
                BaseSalary = baseSalary,
                TotalAllowances = totalAllowances,
                TotalFixedDeductions = totalDeductions,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LateDays = lateDays,
                AbsenceDeduction = absenceDeduction,
                LateDeduction = lateDeduction,
                OvertimeAmount = overtimeAmount,
                BonusAmount = bonusAmount,
                PenaltyAmount = penaltyAmount,
                AdvanceDeduction = advanceDeduction,
                LoanDeduction = loanDeduction,
                GrossSalary = grossSalary,
                TotalDeductions = totalDeductionsAll,
                NetSalary = netSalary
            };
            await _payrollItemRepo.AddAsync(item);

            payroll.TotalBaseSalary += baseSalary;
            payroll.TotalAllowances += totalAllowances;
            payroll.TotalDeductions += totalDeductions;
            payroll.TotalBonuses += bonusAmount;
            payroll.TotalPenalties += penaltyAmount;
            payroll.TotalOvertimeAmount += overtimeAmount;
            payroll.TotalAbsenceDeductions += absenceDeduction;
            payroll.TotalLateDeductions += lateDeduction;
            payroll.TotalAdvanceDeductions += advanceDeduction;
            payroll.TotalLoanDeductions += loanDeduction;
            payroll.TotalGrossSalary += grossSalary;
            payroll.TotalNetSalary += netSalary;
        }

        payroll.Status = PayrollStatus.Calculated;
        _payrollRepo.Update(payroll);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<HrMonthlyPayrollDto>(payroll);
    }

    public async Task<HrMonthlyPayrollDto?> GetPayrollAsync(int month, int year, int branchId)
    {
        var payroll = await _payrollRepo.Query()
            .Include(p => p.PayrollItems)
            .Include(p => p.Branch)
            .FirstOrDefaultAsync(p => p.Month == month && p.Year == year && p.BranchId == branchId);
        return payroll == null ? null : _mapper.Map<HrMonthlyPayrollDto>(payroll);
    }

    public async Task<List<HrMonthlyPayrollDto>> GetPayrollListAsync(int? year = null)
    {
        var query = _payrollRepo.Query().Include(p => p.Branch).AsQueryable();
        if (year.HasValue) query = query.Where(p => p.Year == year.Value);
        var items = await query.OrderByDescending(p => p.Year).ThenByDescending(p => p.Month).ToListAsync();
        return _mapper.Map<List<HrMonthlyPayrollDto>>(items);
    }

    public async Task ApprovePayrollAsync(int payrollId, string approvedBy)
    {
        var payroll = await _payrollRepo.GetByIdAsync(payrollId)
            ?? throw new KeyNotFoundException($"Payroll with ID {payrollId} not found.");
        payroll.Status = PayrollStatus.Approved;
        payroll.ApprovedBy = approvedBy;
        payroll.ApprovedDate = DateTime.UtcNow;
        _payrollRepo.Update(payroll);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkPayrollPaidAsync(int payrollId)
    {
        var payroll = await _payrollRepo.Query()
            .Include(p => p.PayrollItems)
            .FirstOrDefaultAsync(p => p.Id == payrollId)
            ?? throw new KeyNotFoundException($"Payroll with ID {payrollId} not found.");
        payroll.Status = PayrollStatus.Paid;
        payroll.PaidDate = DateTime.UtcNow;
        foreach (var item in payroll.PayrollItems)
        {
            item.IsPaid = true;
            item.PaidDate = DateTime.UtcNow;
        }
        _payrollRepo.Update(payroll);
        await _unitOfWork.SaveChangesAsync();
    }

    // Advances
    public async Task<List<HrSalaryAdvanceDto>> GetAdvancesAsync(AdvanceStatus? status = null)
    {
        var query = _advanceRepo.Query().Include(a => a.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        var items = await query.OrderByDescending(a => a.RequestDate).ToListAsync();
        return _mapper.Map<List<HrSalaryAdvanceDto>>(items);
    }

    public async Task<List<HrSalaryAdvanceDto>> GetAdvancesBySchoolIdAsync(int schoolId, AdvanceStatus? status = null)
    {
        var query = _advanceRepo.Query().Where(a => a.SchoolId == schoolId).Include(a => a.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        var items = await query.OrderByDescending(a => a.RequestDate).ToListAsync();
        return _mapper.Map<List<HrSalaryAdvanceDto>>(items);
    }

    public async Task<HrSalaryAdvanceDto> CreateAdvanceAsync(HrSalaryAdvanceDto dto)
    {
        var entity = _mapper.Map<HrSalaryAdvance>(dto);
        entity.Id = 0;
        entity.Status = AdvanceStatus.Pending;
        entity.RequestDate = DateTime.UtcNow;
        await _advanceRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrSalaryAdvanceDto>(entity);
    }

    public async Task ApproveAdvanceAsync(int id, string approvedBy, decimal approvedAmount, int deductionMonths)
    {
        var entity = await _advanceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Advance with ID {id} not found.");
        entity.Status = AdvanceStatus.Approved;
        entity.ApprovedBy = approvedBy;
        entity.ApprovalDate = DateTime.UtcNow;
        entity.ApprovedAmount = approvedAmount;
        entity.DeductionMonths = deductionMonths;
        entity.MonthlyDeduction = approvedAmount / deductionMonths;
        entity.RemainingAmount = approvedAmount;
        _advanceRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAdvanceAsync(int id, string rejectedBy, string reason)
    {
        var entity = await _advanceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Advance with ID {id} not found.");
        entity.Status = AdvanceStatus.Rejected;
        entity.ApprovedBy = rejectedBy;
        entity.RejectionReason = reason;
        _advanceRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // Loans
    public async Task<List<HrEmployeeLoanDto>> GetLoansAsync(int? employeeId = null)
    {
        var query = _loanRepo.Query().Include(l => l.Employee).AsQueryable();
        if (employeeId.HasValue) query = query.Where(l => l.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(l => l.LoanDate).ToListAsync();
        return _mapper.Map<List<HrEmployeeLoanDto>>(items);
    }

    public async Task<List<HrEmployeeLoanDto>> GetLoansBySchoolIdAsync(int schoolId, int? employeeId = null)
    {
        var query = _loanRepo.Query().Where(l => l.SchoolId == schoolId).Include(l => l.Employee).AsQueryable();
        if (employeeId.HasValue) query = query.Where(l => l.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(l => l.LoanDate).ToListAsync();
        return _mapper.Map<List<HrEmployeeLoanDto>>(items);
    }

    public async Task<HrEmployeeLoanDto> CreateLoanAsync(HrEmployeeLoanDto dto)
    {
        var entity = _mapper.Map<HrEmployeeLoan>(dto);
        entity.Id = 0;
        entity.TotalRepayment = dto.LoanAmount * (1 + dto.InterestRate / 100);
        entity.MonthlyInstallment = entity.TotalRepayment / dto.RepaymentMonths;
        entity.RemainingBalance = entity.TotalRepayment;
        entity.Status = LoanStatus.Active;
        await _loanRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEmployeeLoanDto>(entity);
    }

    // Bonuses
    public async Task<List<HrBonusDto>> GetBonusesAsync(int? month = null, int? year = null)
    {
        var query = _bonusRepo.Query().Include(b => b.Employee).AsQueryable();
        if (month.HasValue) query = query.Where(b => b.Month == month.Value);
        if (year.HasValue) query = query.Where(b => b.Year == year.Value);
        var items = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return _mapper.Map<List<HrBonusDto>>(items);
    }

    public async Task<List<HrBonusDto>> GetBonusesBySchoolIdAsync(int schoolId, int? month = null, int? year = null)
    {
        var query = _bonusRepo.Query().Where(b => b.SchoolId == schoolId).Include(b => b.Employee).AsQueryable();
        if (month.HasValue) query = query.Where(b => b.Month == month.Value);
        if (year.HasValue) query = query.Where(b => b.Year == year.Value);
        var items = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return _mapper.Map<List<HrBonusDto>>(items);
    }

    public async Task<HrBonusDto> CreateBonusAsync(HrBonusDto dto)
    {
        var entity = _mapper.Map<HrBonus>(dto);
        entity.Id = 0;
        entity.Status = BonusStatus.Pending;
        await _bonusRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrBonusDto>(entity);
    }

    public async Task ApproveBonusAsync(int id, string approvedBy)
    {
        var entity = await _bonusRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Bonus with ID {id} not found.");
        entity.Status = BonusStatus.Approved;
        entity.ApprovedBy = approvedBy;
        entity.ApprovalDate = DateTime.UtcNow;
        _bonusRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // Penalties
    public async Task<List<HrPenaltyDto>> GetPenaltiesAsync(int? month = null, int? year = null)
    {
        var query = _penaltyRepo.Query().Include(p => p.Employee).AsQueryable();
        if (month.HasValue) query = query.Where(p => p.Month == month.Value);
        if (year.HasValue) query = query.Where(p => p.Year == year.Value);
        var items = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return _mapper.Map<List<HrPenaltyDto>>(items);
    }

    public async Task<List<HrPenaltyDto>> GetPenaltiesBySchoolIdAsync(int schoolId, int? month = null, int? year = null)
    {
        var query = _penaltyRepo.Query().Where(p => p.SchoolId == schoolId).Include(p => p.Employee).AsQueryable();
        if (month.HasValue) query = query.Where(p => p.Month == month.Value);
        if (year.HasValue) query = query.Where(p => p.Year == year.Value);
        var items = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return _mapper.Map<List<HrPenaltyDto>>(items);
    }

    public async Task<HrPenaltyDto> CreatePenaltyAsync(HrPenaltyDto dto)
    {
        var entity = _mapper.Map<HrPenalty>(dto);
        entity.Id = 0;
        entity.Status = PenaltyStatus.Pending;
        await _penaltyRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPenaltyDto>(entity);
    }

    public async Task ApprovePenaltyAsync(int id, string approvedBy)
    {
        var entity = await _penaltyRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Penalty with ID {id} not found.");
        entity.Status = PenaltyStatus.Approved;
        entity.ApprovedBy = approvedBy;
        entity.ApprovalDate = DateTime.UtcNow;
        _penaltyRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private HrSalaryDetailDto MapSalaryDetail(HrSalaryDetail s)
    {
        var totalAllowances = s.Allowances.Where(a => a.IsActive).Sum(a => a.CalculatedAmount);
        var totalDeductions = s.Deductions.Where(d => d.IsActive).Sum(d => d.CalculatedAmount);
        return new HrSalaryDetailDto
        {
            Id = s.Id, EmployeeId = s.EmployeeId, EmployeeName = s.Employee?.FullName,
            BaseSalary = s.BaseSalary, Currency = s.Currency, SalaryType = s.SalaryType,
            EffectiveDate = s.EffectiveDate, EndDate = s.EndDate, IsCurrent = s.IsCurrent,
            Notes = s.Notes, TotalAllowances = totalAllowances, TotalDeductions = totalDeductions,
            NetSalary = s.BaseSalary + totalAllowances - totalDeductions,
            Allowances = s.Allowances.Select(a => new HrSalaryAllowanceDto
            {
                Id = a.Id, SalaryDetailId = a.SalaryDetailId, AllowanceTypeId = a.AllowanceTypeId,
                AllowanceName = a.AllowanceType?.AllowanceName, CalculationType = a.CalculationType,
                Value = a.Value, CalculatedAmount = a.CalculatedAmount, IsActive = a.IsActive
            }).ToList(),
            Deductions = s.Deductions.Select(d => new HrSalaryDeductionDto
            {
                Id = d.Id, SalaryDetailId = d.SalaryDetailId, DeductionTypeId = d.DeductionTypeId,
                DeductionName = d.DeductionType?.DeductionName, CalculationType = d.CalculationType,
                Value = d.Value, CalculatedAmount = d.CalculatedAmount, IsActive = d.IsActive
            }).ToList()
        };
    }
}
