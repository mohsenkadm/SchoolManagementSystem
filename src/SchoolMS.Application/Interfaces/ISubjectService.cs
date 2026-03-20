using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync();
    Task<List<SubjectDto>> GetBySchoolIdAsync(int schoolId);
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<SubjectDto> CreateAsync(SubjectDto dto);
    Task<SubjectDto> UpdateAsync(SubjectDto dto);
    Task DeleteAsync(int id);
}

