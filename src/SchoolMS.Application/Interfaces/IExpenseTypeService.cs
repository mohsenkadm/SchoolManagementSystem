using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IExpenseTypeService
{
    Task<List<ExpenseTypeDto>> GetAllAsync();
    Task<List<ExpenseTypeDto>> GetBySchoolIdAsync(int schoolId);
    Task<ExpenseTypeDto?> GetByIdAsync(int id);
    Task<ExpenseTypeDto> CreateAsync(ExpenseTypeDto dto);
    Task<ExpenseTypeDto> UpdateAsync(ExpenseTypeDto dto);
    Task DeleteAsync(int id);
}

