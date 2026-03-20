using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly IRepository<ExpenseType> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExpenseTypeService(IRepository<ExpenseType> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<ExpenseTypeDto>> GetAllAsync()
        => _mapper.Map<List<ExpenseTypeDto>>(await _repository.Query().Include(e => e.School).ToListAsync());

    public async Task<List<ExpenseTypeDto>> GetBySchoolIdAsync(int schoolId)
        => _mapper.Map<List<ExpenseTypeDto>>(await _repository.Query().Where(e => e.SchoolId == schoolId).Include(e => e.School).ToListAsync());
    public async Task<ExpenseTypeDto?> GetByIdAsync(int id)
    { var e = await _repository.GetByIdAsync(id); return e == null ? null : _mapper.Map<ExpenseTypeDto>(e); }
    public async Task<ExpenseTypeDto> CreateAsync(ExpenseTypeDto dto)
    { var e = _mapper.Map<ExpenseType>(dto); e.Id = 0; await _repository.AddAsync(e); await _unitOfWork.SaveChangesAsync(); return _mapper.Map<ExpenseTypeDto>(e); }
    public async Task<ExpenseTypeDto> UpdateAsync(ExpenseTypeDto dto)
    { var e = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException(); e.TypeName = dto.TypeName; e.Description = dto.Description; e.SchoolId = dto.SchoolId; _repository.Update(e); await _unitOfWork.SaveChangesAsync(); return _mapper.Map<ExpenseTypeDto>(e); }
    public async Task DeleteAsync(int id)
    { var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException(); e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; _repository.Update(e); await _unitOfWork.SaveChangesAsync(); }
}

