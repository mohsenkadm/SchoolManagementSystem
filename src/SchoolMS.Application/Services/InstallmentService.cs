using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class InstallmentService : IInstallmentService
{
    private readonly IRepository<FeeInstallment> _feeRepo;
    private readonly IRepository<InstallmentPayment> _payRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InstallmentService(IRepository<FeeInstallment> feeRepo, IRepository<InstallmentPayment> payRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    { _feeRepo = feeRepo; _payRepo = payRepo; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<FeeInstallmentDto>> GetAllAsync()
    {
        var items = await _feeRepo.Query()
            .Include(f => f.Student).Include(f => f.AcademicYear).Include(f => f.InstallmentPayments)
            .Include(f => f.School)
            .ToListAsync();
        return _mapper.Map<List<FeeInstallmentDto>>(items);
    }

    public async Task<List<FeeInstallmentDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _feeRepo.Query().Where(f => f.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(f => f.Student.BranchId == branchId.Value);
        var items = await query
            .Include(f => f.Student).Include(f => f.AcademicYear).Include(f => f.InstallmentPayments)
            .Include(f => f.School)
            .ToListAsync();
        return _mapper.Map<List<FeeInstallmentDto>>(items);
    }

    public async Task<List<FeeInstallmentDto>> GetByStudentIdAsync(int studentId, int schoolId)
    {
        var items = await _feeRepo.Query()
            .Where(f => f.StudentId == studentId && f.SchoolId == schoolId)
            .Include(f => f.Student).Include(f => f.AcademicYear).Include(f => f.InstallmentPayments)
            .Include(f => f.School)
            .ToListAsync();
        return _mapper.Map<List<FeeInstallmentDto>>(items);
    }

    public async Task<List<FeeInstallmentDto>> GetByParentChildrenAsync(int parentId, int schoolId)
    {
        var items = await _feeRepo.Query()
            .Where(f => f.SchoolId == schoolId && f.Student.ParentId == parentId)
            .Include(f => f.Student).Include(f => f.AcademicYear).Include(f => f.InstallmentPayments)
            .Include(f => f.School)
            .ToListAsync();
        return _mapper.Map<List<FeeInstallmentDto>>(items);
    }

    public async Task<FeeInstallmentDto?> GetByIdAsync(int id)
    {
        var entity = await _feeRepo.Query()
            .Include(f => f.Student).Include(f => f.AcademicYear).Include(f => f.InstallmentPayments)
            .Include(f => f.School)
            .FirstOrDefaultAsync(f => f.Id == id);
        return entity == null ? null : _mapper.Map<FeeInstallmentDto>(entity);
    }

    public async Task<FeeInstallmentDto> CreateAsync(FeeInstallmentDto dto)
    {
        var entity = new FeeInstallment
        {
            StudentId = dto.StudentId,
            TotalAmount = dto.TotalAmount,
            NumberOfPayments = dto.NumberOfPayments,
            AcademicYearId = dto.AcademicYearId,
            DefaultPaymentPlanId = dto.DefaultPaymentPlanId,
            SchoolId = dto.SchoolId
        };
        await _feeRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var amountPerPayment = dto.TotalAmount / dto.NumberOfPayments;
        for (int i = 0; i < dto.NumberOfPayments; i++)
        {
            var payment = new InstallmentPayment
            {
                FeeInstallmentId = entity.Id,
                PaymentNumber = i + 1,
                Amount = amountPerPayment,
                DueDate = DateTime.UtcNow.AddMonths(i + 1),
                Status = PaymentStatus.Pending,
                SchoolId = entity.SchoolId
            };
            await _payRepo.AddAsync(payment);
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<FeeInstallmentDto>(entity);
    }

    public async Task<FeeInstallmentDto> UpdateAsync(FeeInstallmentDto dto)
    {
        var entity = await _feeRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.TotalAmount = dto.TotalAmount; entity.NumberOfPayments = dto.NumberOfPayments;
        entity.SchoolId = dto.SchoolId;
        _feeRepo.Update(entity);
        return _mapper.Map<FeeInstallmentDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _feeRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _feeRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task RecordPaymentAsync(int paymentId)
    {
        var payment = await _payRepo.GetByIdAsync(paymentId) ?? throw new KeyNotFoundException();
        payment.PaidDate = DateTime.UtcNow;
        payment.Status = PaymentStatus.Paid;
        _payRepo.Update(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelPaymentAsync(int paymentId)
    {
        var payment = await _payRepo.GetByIdAsync(paymentId) ?? throw new KeyNotFoundException();
        payment.PaidDate = null;
        payment.Status = payment.DueDate < DateTime.UtcNow.Date ? PaymentStatus.Overdue : PaymentStatus.Pending;
        _payRepo.Update(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<InstallmentPaymentDto>> GetAllPaymentsAsync(PaymentStatus? statusFilter = null)
    {
        var query = _payRepo.Query()
            .Include(p => p.FeeInstallment).ThenInclude(f => f.Student).ThenInclude(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(p => p.FeeInstallment).ThenInclude(f => f.Student).ThenInclude(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(p => p.FeeInstallment).ThenInclude(f => f.AcademicYear)
            .Include(p => p.School)
            .AsQueryable();
        if (statusFilter.HasValue)
            query = query.Where(p => p.Status == statusFilter.Value);
        var items = await query.OrderBy(p => p.DueDate).ToListAsync();
        return _mapper.Map<List<InstallmentPaymentDto>>(items);
    }

    public async Task<List<InstallmentPaymentDto>> GetPaymentsBySchoolIdAsync(int schoolId, PaymentStatus? statusFilter = null)
    {
        var query = _payRepo.Query()
            .Where(p => p.SchoolId == schoolId)
            .Include(p => p.FeeInstallment).ThenInclude(f => f.Student).ThenInclude(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(p => p.FeeInstallment).ThenInclude(f => f.Student).ThenInclude(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(p => p.FeeInstallment).ThenInclude(f => f.AcademicYear)
            .Include(p => p.School)
            .AsQueryable();
        if (statusFilter.HasValue)
            query = query.Where(p => p.Status == statusFilter.Value);
        var items = await query.OrderBy(p => p.DueDate).ToListAsync();
        return _mapper.Map<List<InstallmentPaymentDto>>(items);
    }

    public async Task<List<InstallmentPaymentDto>> GetOverdueAsync()
    {
        var items = await _payRepo.Query()
            .Where(p => p.Status != PaymentStatus.Paid && p.DueDate < DateTime.UtcNow.Date)
            .ToListAsync();
        foreach (var item in items)
        {
            if (item.Status != PaymentStatus.Overdue)
            {
                item.Status = PaymentStatus.Overdue;
                _payRepo.Update(item);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<List<InstallmentPaymentDto>>(items);
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Installments");
        var ws = workbook.Worksheet("Installments");
        ws.Cell(1, 1).Value = "Student"; ws.Cell(1, 2).Value = "Total"; ws.Cell(1, 3).Value = "Payments";
        ws.Cell(1, 4).Value = "Year";
        ws.Range("A1:D1").Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].StudentName;
            ws.Cell(i + 2, 2).Value = items[i].TotalAmount;
            ws.Cell(i + 2, 3).Value = items[i].NumberOfPayments;
            ws.Cell(i + 2, 4).Value = items[i].AcademicYearName;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream); return stream.ToArray();
    }

    public async Task<byte[]> ExportPaymentsToExcelAsync(PaymentStatus? statusFilter = null)
    {
        var items = await GetAllPaymentsAsync(statusFilter);
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("Payments");
        var headers = new[] { "Student", "Grade", "Division", "Phone", "Payment #", "Amount", "Due Date", "Paid Date", "Status" };
        for (int h = 0; h < headers.Length; h++) ws.Cell(1, h + 1).Value = headers[h];
        ws.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].StudentName;
            ws.Cell(i + 2, 2).Value = items[i].GradeName;
            ws.Cell(i + 2, 3).Value = items[i].DivisionName;
            ws.Cell(i + 2, 4).Value = items[i].Phone;
            ws.Cell(i + 2, 5).Value = items[i].PaymentNumber;
            ws.Cell(i + 2, 6).Value = items[i].Amount;
            ws.Cell(i + 2, 7).Value = items[i].DueDate.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 8).Value = items[i].PaidDate?.ToString("yyyy-MM-dd") ?? "";
            ws.Cell(i + 2, 9).Value = items[i].Status.ToString();
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms); return ms.ToArray();
    }

    public async Task<byte[]> ExportStudentSummaryToExcelAsync()
    {
        var all = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("StudentSummary");
        ws.Cell(1, 1).Value = "Student"; ws.Cell(1, 2).Value = "Total"; ws.Cell(1, 3).Value = "Paid"; ws.Cell(1, 4).Value = "Unpaid";
        ws.Range("A1:D1").Style.Font.Bold = true;
        int row = 2;
        foreach (var item in all)
        {
            var paid = item.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
            ws.Cell(row, 1).Value = item.StudentName;
            ws.Cell(row, 2).Value = item.TotalAmount;
            ws.Cell(row, 3).Value = paid;
            ws.Cell(row, 4).Value = item.TotalAmount - paid;
            row++;
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms); return ms.ToArray();
    }
}

