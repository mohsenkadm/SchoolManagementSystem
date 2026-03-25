using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ICourseVideoService
{
    Task<List<CourseVideoDto>> GetAllByCourseAsync(int courseId);
    Task<List<CourseVideoDto>> GetAllByCourseForStudentAsync(int courseId, int studentId);
    Task<List<CourseVideoDto>> GetFreeVideosByCourseAsync(int courseId);
    Task<CourseVideoDto?> GetByIdAsync(int id);
    Task<CourseVideoDto> CreateAsync(CreateCourseVideoDto dto);
    Task<CourseVideoDto> UpdateAsync(CourseVideoDto dto);
    Task DeleteAsync(int id);
    Task<bool> ToggleLikeAsync(int courseVideoId, int studentId);
    Task MarkSeenAsync(int courseVideoId, int studentId);
    Task<double> RateVideoAsync(int courseVideoId, int studentId, int rating);

    // Video Comments
    Task<List<VideoCommentDto>> GetCommentsByVideoIdAsync(int courseVideoId);
    Task<VideoCommentDto> AddCommentAsync(CreateVideoCommentDto dto);

    // Video Quiz Questions
    Task<List<VideoQuizQuestionDto>> GetQuizQuestionsByVideoAsync(int courseVideoId);
    Task<VideoQuizQuestionDto> CreateQuizQuestionAsync(CreateVideoQuizQuestionDto dto);
    Task<VideoQuizQuestionDto> UpdateQuizQuestionAsync(VideoQuizQuestionDto dto);
    Task DeleteQuizQuestionAsync(int id);

    // Video Quiz Answers
    Task<VideoQuizAnswerDto> SubmitQuizAnswerAsync(SubmitVideoQuizAnswerDto dto);
    Task<List<VideoQuizAnswerDto>> GetStudentAnswersAsync(int courseVideoId, int studentId);
    Task<List<VideoQuizAnswerDto>> GetAllAnswersByVideoAsync(int courseVideoId);

    // Video Notes
    Task<VideoNoteDto?> GetNoteAsync(int courseVideoId, int studentId);
    Task<VideoNoteDto> SaveNoteAsync(SaveVideoNoteDto dto);
}
