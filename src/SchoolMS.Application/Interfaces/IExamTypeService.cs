using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IExamTypeService
{
    Task<List<ExamTypeDto>> GetAllAsync();
    Task<List<ExamTypeDto>> GetBySchoolIdAsync(int schoolId);
    Task<ExamTypeDto?> GetByIdAsync(int id);
    Task<ExamTypeDto> CreateAsync(ExamTypeDto dto);
    Task<ExamTypeDto> UpdateAsync(ExamTypeDto dto);
    Task DeleteAsync(int id);
}

