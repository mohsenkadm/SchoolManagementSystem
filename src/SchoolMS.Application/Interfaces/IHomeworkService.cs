using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IHomeworkService
{
    Task<List<HomeworkDto>> GetAllAsync();
    Task<List<HomeworkDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? classRoomId = null,
        int? subjectId = null, int? teacherId = null);
    Task<List<HomeworkDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId);
    Task<HomeworkDto?> GetByIdAsync(int id);
    Task<HomeworkDto> CreateAsync(HomeworkDto dto);
    Task<HomeworkDto> UpdateAsync(HomeworkDto dto);
    Task DeleteAsync(int id);
    Task AddAttachmentsAsync(int homeworkId, List<HomeworkAttachmentDto> attachments);
}

