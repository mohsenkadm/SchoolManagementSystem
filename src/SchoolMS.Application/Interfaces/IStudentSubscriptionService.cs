using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStudentSubscriptionService
{
    Task<List<StudentSubscriptionDto>> GetAllAsync();
    Task<List<StudentSubscriptionDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<StudentSubscriptionDto>> GetByStudentIdAsync(int studentId, int schoolId);
    Task<StudentSubscriptionDto?> GetByIdAsync(int id);
    Task<StudentSubscriptionDto> CreateAsync(StudentSubscriptionDto dto);
    Task<StudentSubscriptionDto> UpdateAsync(StudentSubscriptionDto dto);
    Task UpdateStatusAsync(int id, SchoolMS.Domain.Enums.SubscriptionStatus status);
    Task MarkAsPaidAsync(int id);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}
