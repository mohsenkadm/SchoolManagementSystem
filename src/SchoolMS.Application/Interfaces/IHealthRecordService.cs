using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IHealthRecordService
{
    Task<List<HealthRecordDto>> GetAllAsync(int? academicYearId = null);
    Task<List<HealthRecordDto>> GetBySchoolIdAsync(int schoolId, int? studentId = null, int? academicYearId = null);
    Task<List<HealthRecordDto>> GetByStudentIdAsync(int studentId, int? academicYearId = null);
    Task<List<HealthRecordDto>> GetByParentChildrenAsync(int parentId, int schoolId, int? academicYearId = null);
    Task<HealthRecordDto?> GetByIdAsync(int id);
    Task<HealthRecordDto> CreateAsync(HealthRecordDto dto);
    Task<HealthRecordDto> UpdateAsync(HealthRecordDto dto);
    Task DeleteAsync(int id);
}

