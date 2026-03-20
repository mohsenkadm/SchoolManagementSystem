using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllAsync();
    Task<List<CourseDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<CourseDto>> GetByTeacherIdAsync(int teacherId);
    Task<List<CourseDto>> GetBySubjectIdsAsync(List<int> subjectIds);
    Task<List<CourseDto>> GetTopCoursesAsync(int schoolId, int count = 10);
    Task<CourseDto?> GetByIdAsync(int id);
    Task<CourseDto> CreateAsync(CreateCourseDto dto);
    Task<CourseDto> UpdateAsync(CourseDto dto);
    Task DeleteAsync(int id);
}
