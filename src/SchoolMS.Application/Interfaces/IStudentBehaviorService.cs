using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStudentBehaviorService
{
    Task<List<StudentBehaviorDto>> GetAllAsync();
    Task<List<StudentBehaviorDto>> GetByStudentIdAsync(int studentId);
    Task<List<StudentBehaviorDto>> GetByParentChildrenAsync(int parentId, int schoolId);
    Task<StudentBehaviorDto?> GetByIdAsync(int id);
    Task<StudentBehaviorDto> CreateAsync(StudentBehaviorDto dto);
    Task<StudentBehaviorDto> UpdateAsync(StudentBehaviorDto dto);
    Task DeleteAsync(int id);
}

