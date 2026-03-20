using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrEndOfServiceService : IHrEndOfServiceService
{
    private readonly IRepository<HrEndOfService> _repository;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IRepository<HrSalaryDetail> _salaryRepo;
    private readonly IRepository<HrLeaveBalance> _leaveBalanceRepo;
    private readonly IRepository<HrSalaryAdvance> _advanceRepo;
    private readonly IRepository<HrEmployeeLoan> _loanRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrEndOfServiceService(IRepository<HrEndOfService> repository, IRepository<HrEmployee> employeeRepo,
        IRepository<HrSalaryDetail> salaryRepo, IRepository<HrLeaveBalance> leaveBalanceRepo,
        IRepository<HrSalaryAdvance> advanceRepo, IRepository<HrEmployeeLoan> loanRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository; _employeeRepo = employeeRepo; _salaryRepo = salaryRepo;
        _leaveBalanceRepo = leaveBalanceRepo; _advanceRepo = advanceRepo; _loanRepo = loanRepo;
        _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<HrEndOfServiceDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(e => e.Employee).OrderByDescending(e => e.RequestDate).ToListAsync();
        return items.Select(e => new HrEndOfServiceDto
        {
            Id = e.Id, EmployeeId = e.EmployeeId, EmployeeName = e.Employee?.FullName,
            Type = e.Type, RequestDate = e.RequestDate, EffectiveDate = e.EffectiveDate,
            LastWorkingDay = e.LastWorkingDay, Reason = e.Reason,
            TotalServiceYears = e.TotalServiceYears, TotalServiceMonths = e.TotalServiceMonths,
            LastBaseSalary = e.LastBaseSalary, EndOfServiceBenefit = e.EndOfServiceBenefit,
            UnusedLeaveCompensation = e.UnusedLeaveCompensation, TotalEntitlements = e.TotalEntitlements,
            TotalDeductions = e.TotalDeductions, FinalSettlementAmount = e.FinalSettlementAmount,
            IsSettled = e.IsSettled, Status = e.Status, AllClearancesCompleted = e.AllClearancesCompleted
        }).ToList();
    }

    public async Task<HrEndOfServiceDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query().Include(e => e.Employee).FirstOrDefaultAsync(e => e.Id == id);
        return entity == null ? null : _mapper.Map<HrEndOfServiceDto>(entity);
    }

    public async Task<HrEndOfServiceDto> CreateAsync(HrEndOfServiceDto dto)
    {
        var entity = _mapper.Map<HrEndOfService>(dto); entity.Id = 0;
        entity.Status = EndOfServiceStatus.Requested; entity.RequestDate = DateTime.UtcNow;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEndOfServiceDto>(entity);
    }

    public async Task<HrEndOfServiceDto> CalculateSettlementAsync(int employeeId, EndOfServiceType type, DateTime effectiveDate)
    {
        var employee = await _employeeRepo.GetByIdAsync(employeeId)
            ?? throw new KeyNotFoundException($"Employee {employeeId} not found.");

        var serviceDuration = effectiveDate - employee.HireDate;
        var totalYears = (int)(serviceDuration.TotalDays / 365.25);
        var totalMonths = (int)((serviceDuration.TotalDays % 365.25) / 30.44);

        var salary = await _salaryRepo.Query().FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.IsCurrent);
        var baseSalary = salary?.BaseSalary ?? 0;

        // End of service benefit calculation (simplified labor law)
        decimal benefit;
        if (totalYears <= 5)
            benefit = (baseSalary / 2) * totalYears;
        else
            benefit = (baseSalary / 2) * 5 + baseSalary * (totalYears - 5);

        // Unused leave compensation
        var leaveBalances = await _leaveBalanceRepo.Query()
            .Where(b => b.EmployeeId == employeeId && b.Year == DateTime.UtcNow.Year)
            .ToListAsync();
        var unusedDays = leaveBalances.Sum(b => b.Remaining);
        var dailySalary = baseSalary / 30;
        var leaveCompensation = unusedDays * dailySalary;

        // Pending deductions
        var pendingAdvances = await _advanceRepo.Query()
            .Where(a => a.EmployeeId == employeeId && a.Status == AdvanceStatus.Approved && a.RemainingAmount > 0)
            .SumAsync(a => a.RemainingAmount);

        var pendingLoans = await _loanRepo.Query()
            .Where(l => l.EmployeeId == employeeId && l.Status == LoanStatus.Active)
            .SumAsync(l => l.RemainingBalance);

        var totalEntitlements = benefit + leaveCompensation;
        var totalDeductions = pendingAdvances + pendingLoans;

        return new HrEndOfServiceDto
        {
            EmployeeId = employeeId, EmployeeName = employee.FullName, Type = type,
            EffectiveDate = effectiveDate, TotalServiceYears = totalYears,
            TotalServiceMonths = totalMonths, LastBaseSalary = baseSalary,
            EndOfServiceBenefit = benefit, UnusedLeaveCompensation = leaveCompensation,
            TotalEntitlements = totalEntitlements, TotalDeductions = totalDeductions,
            FinalSettlementAmount = totalEntitlements - totalDeductions
        };
    }

    public async Task ApproveAsync(int id, string approvedBy)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"EOS {id} not found.");
        entity.Status = EndOfServiceStatus.Approved; entity.ApprovedBy = approvedBy;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();

        var employee = await _employeeRepo.GetByIdAsync(entity.EmployeeId);
        if (employee != null)
        {
            employee.Status = entity.Type == EndOfServiceType.Resignation ? HrEmployeeStatus.Resigned
                : entity.Type == EndOfServiceType.Retirement ? HrEmployeeStatus.Retired
                : HrEmployeeStatus.Terminated;
            _employeeRepo.Update(employee);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkSettledAsync(int id, string paymentMethod, string paymentReference)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"EOS {id} not found.");
        entity.Status = EndOfServiceStatus.Settled; entity.IsSettled = true;
        entity.SettlementDate = DateTime.UtcNow; entity.PaymentMethod = paymentMethod;
        entity.PaymentReference = paymentReference;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }
}
