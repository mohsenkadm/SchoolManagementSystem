using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IAcademicYearService
{
    Task<List<AcademicYearDto>> GetAllAsync(int schoolId);
    Task<AcademicYearDto?> GetByIdAsync(int id);
    Task<AcademicYearDto?> GetCurrentAsync(int schoolId);
    Task<AcademicYearDto> CreateAsync(AcademicYearDto dto);
    Task<AcademicYearDto> UpdateAsync(AcademicYearDto dto);
    Task DeleteAsync(int id);
}

