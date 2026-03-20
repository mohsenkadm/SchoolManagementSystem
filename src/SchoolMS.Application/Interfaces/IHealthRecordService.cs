using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IHealthRecordService
{
    Task<List<HealthRecordDto>> GetAllAsync();
    Task<List<HealthRecordDto>> GetBySchoolIdAsync(int schoolId, int? studentId = null);
    Task<List<HealthRecordDto>> GetByStudentIdAsync(int studentId);
    Task<List<HealthRecordDto>> GetByParentChildrenAsync(int parentId, int schoolId);
    Task<HealthRecordDto?> GetByIdAsync(int id);
    Task<HealthRecordDto> CreateAsync(HealthRecordDto dto);
    Task<HealthRecordDto> UpdateAsync(HealthRecordDto dto);
    Task DeleteAsync(int id);
}

