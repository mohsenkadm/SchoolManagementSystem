using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface ITeacherEarningService
{
    Task<List<TeacherEarningDto>> GetAllAsync();
    Task<List<TeacherEarningDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<TeacherEarningDto>> GetByTeacherIdAsync(int teacherId, int schoolId);
    Task<TeacherEarningDto?> GetByIdAsync(int id);
    Task RecordEarningForSubscriptionAsync(int studentSubscriptionId);
    Task UpdateStatusAsync(int id, TeacherEarningStatus status);
    Task MarkAsPaidAsync(int id);
    Task DeleteAsync(int id);
    Task<List<TeacherEarningSummaryDto>> GetSummaryBySchoolIdAsync(int schoolId);
    Task<TeacherEarningSummaryDto?> GetTeacherSummaryAsync(int teacherId, int schoolId);
    Task<byte[]> ExportToExcelAsync(int? schoolId = null, int? teacherId = null);
}
