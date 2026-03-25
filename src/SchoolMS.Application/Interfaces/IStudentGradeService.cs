using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IStudentGradeService
{
    Task<List<StudentGradeDto>> GetAllAsync();
    Task<List<StudentGradeDto>> GetBySchoolIdAsync(int schoolId, int? subjectId = null, int? examTypeId = null, int? academicYearId = null);
    Task<List<StudentGradeDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId, int? subjectId = null, int? examTypeId = null, int? academicYearId = null);
    Task<StudentGradeDto?> GetByIdAsync(int id);
    Task<StudentGradeDto> CreateAsync(StudentGradeDto dto);
    Task<StudentGradeDto> UpdateAsync(StudentGradeDto dto);
    Task DeleteAsync(int id);
    Task BulkCreateAsync(List<StudentGradeDto> dtos);
    Task<byte[]> ExportToExcelAsync();
}

