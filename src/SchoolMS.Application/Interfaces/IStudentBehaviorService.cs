using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStudentBehaviorService
{
    Task<List<StudentBehaviorDto>> GetAllAsync(int? academicYearId = null);
    Task<List<StudentBehaviorDto>> GetByStudentIdAsync(int studentId, int? academicYearId = null);
    Task<List<StudentBehaviorDto>> GetByParentChildrenAsync(int parentId, int schoolId, int? academicYearId = null);
    Task<StudentBehaviorDto?> GetByIdAsync(int id);
    Task<StudentBehaviorDto> CreateAsync(StudentBehaviorDto dto);
    Task<StudentBehaviorDto> UpdateAsync(StudentBehaviorDto dto);
    Task DeleteAsync(int id);
}

