using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IGradeService
{
    Task<List<GradeDto>> GetAllAsync();
    Task<List<GradeDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<GradeDto?> GetByIdAsync(int id);
    Task<GradeDto> CreateAsync(GradeDto dto);
    Task<GradeDto> UpdateAsync(GradeDto dto);
    Task DeleteAsync(int id);
}

