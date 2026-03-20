using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IExamScheduleService
{
    Task<List<ExamScheduleDto>> GetAllAsync();
    Task<List<ExamScheduleDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? examTypeId = null,
        int? classRoomId = null, int? subjectId = null, int? teacherId = null, int? academicYearId = null);
    Task<List<ExamScheduleDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId);
    Task<ExamScheduleDto?> GetByIdAsync(int id);
    Task<ExamScheduleDto> CreateAsync(ExamScheduleDto dto);
    Task CreateBulkAsync(List<ExamScheduleDto> dtos);
    Task<ExamScheduleDto> UpdateAsync(ExamScheduleDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}

