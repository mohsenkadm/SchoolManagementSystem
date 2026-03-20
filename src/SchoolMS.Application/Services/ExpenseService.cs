using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IRepository<Expense> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExpenseService(IRepository<Expense> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<ExpenseDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(e => e.ExpenseType).Include(e => e.Branch).Include(e => e.School).ToListAsync();
        return _mapper.Map<List<ExpenseDto>>(items);
    }

    public async Task<List<ExpenseDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(e => e.SchoolId == schoolId)
            .Include(e => e.ExpenseType).Include(e => e.Branch).Include(e => e.School).ToListAsync();
        return _mapper.Map<List<ExpenseDto>>(items);
    }

    public async Task<ExpenseDto> CreateAsync(ExpenseDto dto)
    {
        var entity = _mapper.Map<Expense>(dto); entity.Id = 0;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ExpenseDto>(entity);
    }

    public async Task<ExpenseDto> UpdateAsync(ExpenseDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.ExpenseTypeId = dto.ExpenseTypeId; entity.Amount = dto.Amount;
        entity.Date = dto.Date; entity.Description = dto.Description; entity.BranchId = dto.BranchId;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ExpenseDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<DataTableResponse<ExpenseDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _repository.Query().Include(e => e.ExpenseType).Include(e => e.Branch).AsQueryable();
        if (request.BranchId.HasValue) query = query.Where(e => e.BranchId == request.BranchId);
        var totalRecords = await query.CountAsync();
        var items = await query.OrderByDescending(e => e.Date)
            .Skip(request.Start).Take(request.Length).ToListAsync();
        return new DataTableResponse<ExpenseDto>
        {
            Draw = request.Draw, RecordsTotal = totalRecords,
            RecordsFiltered = totalRecords, Data = _mapper.Map<List<ExpenseDto>>(items)
        };
    }
}

