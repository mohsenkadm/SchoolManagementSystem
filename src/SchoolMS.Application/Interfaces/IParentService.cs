using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IParentService
{
    Task<List<ParentDto>> GetAllAsync();
    Task<List<ParentDto>> GetBySchoolIdAsync(int schoolId);
    Task<ParentDto?> GetByIdAsync(int id);
    Task<ParentDto> CreateAsync(ParentDto dto);
    Task<ParentDto> UpdateAsync(ParentDto dto);
    Task DeleteAsync(int id);
}

