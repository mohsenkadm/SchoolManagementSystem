using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface ITeacherAssignmentService
{
    Task<List<TeacherAssignmentDto>> GetAllAsync();
    Task<List<TeacherAssignmentDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<TeacherAssignmentDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId);
    Task<TeacherAssignmentDto?> GetByIdAsync(int id);
    Task<TeacherAssignmentDto> CreateAsync(TeacherAssignmentDto dto);
    Task<TeacherAssignmentDto> UpdateAsync(TeacherAssignmentDto dto);
    Task DeleteAsync(int id);
}

