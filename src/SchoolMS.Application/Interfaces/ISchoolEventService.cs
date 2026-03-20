using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ISchoolEventService
{
    Task<List<SchoolEventDto>> GetAllAsync();
    Task<List<SchoolEventDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<SchoolEventDto?> GetByIdAsync(int id);
    Task<SchoolEventDto> CreateAsync(SchoolEventDto dto);
    Task<SchoolEventDto> UpdateAsync(SchoolEventDto dto);
    Task DeleteAsync(int id);
}

