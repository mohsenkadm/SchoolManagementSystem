using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IExpenseService
{
    Task<List<ExpenseDto>> GetAllAsync();
    Task<List<ExpenseDto>> GetBySchoolIdAsync(int schoolId);
    Task<ExpenseDto> CreateAsync(ExpenseDto dto);
    Task<ExpenseDto> UpdateAsync(ExpenseDto dto);
    Task DeleteAsync(int id);
    Task<DataTableResponse<ExpenseDto>> GetDataTableAsync(DataTableRequest request);
}

