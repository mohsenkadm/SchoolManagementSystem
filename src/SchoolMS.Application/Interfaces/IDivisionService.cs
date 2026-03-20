using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IDivisionService
{
    Task<List<DivisionDto>> GetAllAsync();
    Task<List<DivisionDto>> GetBySchoolIdAsync(int schoolId);
    Task<DivisionDto?> GetByIdAsync(int id);
    Task<DivisionDto> CreateAsync(DivisionDto dto);
    Task<DivisionDto> UpdateAsync(DivisionDto dto);
    Task DeleteAsync(int id);
}

