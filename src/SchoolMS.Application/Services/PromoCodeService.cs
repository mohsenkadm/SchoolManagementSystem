using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IRepository<PromoCode> _repository;
    private readonly IRepository<PromoCodeUsage> _usageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PromoCodeService(
        IRepository<PromoCode> repository,
        IRepository<PromoCodeUsage> usageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _usageRepository = usageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PromoCodeDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(p => p.School)
            .Include(p => p.Usages).ThenInclude(u => u.Student)
            .ToListAsync();
        return _mapper.Map<List<PromoCodeDto>>(items);
    }

    public async Task<List<PromoCodeDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(p => p.SchoolId == schoolId)
            .Include(p => p.School)
            .Include(p => p.Usages).ThenInclude(u => u.Student)
            .ToListAsync();
        return _mapper.Map<List<PromoCodeDto>>(items);
    }

    public async Task<PromoCodeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(p => p.School)
            .Include(p => p.Usages).ThenInclude(u => u.Student)
            .FirstOrDefaultAsync(p => p.Id == id);
        return entity == null ? null : _mapper.Map<PromoCodeDto>(entity);
    }

    public async Task<PromoCodeDto?> GetByCodeAsync(string code)
    {
        var entity = await _repository.Query()
            .Include(p => p.School)
            .Include(p => p.Usages).ThenInclude(u => u.Student)
            .FirstOrDefaultAsync(p => p.Code == code);
        return entity == null ? null : _mapper.Map<PromoCodeDto>(entity);
    }

    public async Task<PromoCodeDto> CreateAsync(PromoCodeDto dto)
    {
        var entity = _mapper.Map<PromoCode>(dto);
        entity.Id = 0;
        entity.CurrentUsage = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PromoCodeDto>(entity);
    }

    public async Task<PromoCodeDto> UpdateAsync(PromoCodeDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"PromoCode with ID {dto.Id} not found.");
        entity.Code = dto.Code;
        entity.DiscountType = dto.DiscountType;
        entity.DiscountValue = dto.DiscountValue;
        entity.ExpiryDate = dto.ExpiryDate;
        entity.MaxUsage = dto.MaxUsage;
        entity.IsUnlimited = dto.IsUnlimited;
        entity.IsActive = dto.IsActive;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PromoCodeDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"PromoCode with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<(bool valid, string? error, decimal discountAmount)> ValidateAndCalculateDiscountAsync(
        string code, int studentId, decimal originalAmount)
    {
        var promo = await _repository.Query()
            .Include(p => p.Usages)
            .FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);

        if (promo == null)
            return (false, "Promo code not found.", 0);

        if (!promo.IsActive)
            return (false, "Promo code is not active.", 0);

        if (!promo.IsUnlimited && promo.ExpiryDate.HasValue && promo.ExpiryDate.Value < DateTime.UtcNow)
            return (false, "Promo code has expired.", 0);

        if (!promo.IsUnlimited && promo.MaxUsage > 0 && promo.CurrentUsage >= promo.MaxUsage)
            return (false, "Promo code has reached maximum usage.", 0);

        var alreadyUsed = promo.Usages.Any(u => u.StudentId == studentId && !u.IsDeleted);
        if (!promo.IsUnlimited && alreadyUsed)
            return (false, "This promo code has already been used by this student.", 0);

        decimal discount = promo.DiscountType == DiscountType.Percentage
            ? originalAmount * (promo.DiscountValue / 100m)
            : promo.DiscountValue;

        discount = Math.Min(discount, originalAmount);
        return (true, null, Math.Round(discount, 2));
    }

    public async Task RecordUsageAsync(int promoCodeId, int studentId, int studentSubscriptionId)
    {
        var promo = await _repository.GetByIdAsync(promoCodeId)
            ?? throw new KeyNotFoundException("PromoCode not found.");

        promo.CurrentUsage++;
        _repository.Update(promo);

        var usage = new PromoCodeUsage
        {
            PromoCodeId = promoCodeId,
            StudentId = studentId,
            StudentSubscriptionId = studentSubscriptionId,
            UsedAt = DateTime.UtcNow,
            SchoolId = promo.SchoolId
        };
        await _usageRepository.AddAsync(usage);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("PromoCodes");
        var ws = workbook.Worksheet("PromoCodes");
        ws.Cell(1, 1).Value = "Code";
        ws.Cell(1, 2).Value = "Discount Type";
        ws.Cell(1, 3).Value = "Discount Value";
        ws.Cell(1, 4).Value = "Expiry Date";
        ws.Cell(1, 5).Value = "Max Usage";
        ws.Cell(1, 6).Value = "Current Usage";
        ws.Cell(1, 7).Value = "Unlimited";
        ws.Cell(1, 8).Value = "Active";
        ws.Cell(1, 9).Value = "School";
        ws.Range("A1:I1").Style.Font.Bold = true;
        ws.Range("A1:I1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:I1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].Code;
            ws.Cell(i + 2, 2).Value = items[i].DiscountType.ToString();
            ws.Cell(i + 2, 3).Value = items[i].DiscountValue;
            ws.Cell(i + 2, 4).Value = items[i].ExpiryDate?.ToString("yyyy-MM-dd") ?? "Unlimited";
            ws.Cell(i + 2, 5).Value = items[i].MaxUsage;
            ws.Cell(i + 2, 6).Value = items[i].CurrentUsage;
            ws.Cell(i + 2, 7).Value = items[i].IsUnlimited ? "Yes" : "No";
            ws.Cell(i + 2, 8).Value = items[i].IsActive ? "Yes" : "No";
            ws.Cell(i + 2, 9).Value = items[i].SchoolName;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
