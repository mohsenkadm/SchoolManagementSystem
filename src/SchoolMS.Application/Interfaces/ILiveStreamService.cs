using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ILiveStreamService
{
    Task<List<LiveStreamDto>> GetAllByCourseAsync(int courseId);
    Task<List<LiveStreamDto>> GetAllAsync();
    Task<List<LiveStreamDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<LiveStreamDto>> GetByTeacherIdAsync(int teacherId);
    Task<List<LiveStreamDto>> GetBySubjectIdsAsync(List<int> subjectIds);
    Task<LiveStreamDto?> GetByIdAsync(int id);
    Task<LiveStreamDto> CreateAsync(CreateLiveStreamDto dto);
    Task UpdateStatusAsync(int id, Domain.Enums.LiveStreamStatus status);
    Task DeleteAsync(int id);
    Task MarkSeenAsync(int liveStreamId, int studentId);

    // Live Stream Comments
    Task<List<LiveStreamCommentDto>> GetCommentsByLiveStreamIdAsync(int liveStreamId);
    Task<LiveStreamCommentDto> AddCommentAsync(CreateLiveStreamCommentDto dto);
}
