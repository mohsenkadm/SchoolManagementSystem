using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ICourseVideoService
{
    Task<List<CourseVideoDto>> GetAllByCourseAsync(int courseId);
    Task<CourseVideoDto?> GetByIdAsync(int id);
    Task<CourseVideoDto> CreateAsync(CreateCourseVideoDto dto);
    Task<CourseVideoDto> UpdateAsync(CourseVideoDto dto);
    Task DeleteAsync(int id);
    Task<bool> ToggleLikeAsync(int courseVideoId, int studentId);
    Task MarkSeenAsync(int courseVideoId, int studentId);
    Task<double> RateVideoAsync(int courseVideoId, int studentId, int rating);
}
