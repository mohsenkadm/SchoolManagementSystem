using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStudentDocumentService
{
    Task<List<StudentDocumentDto>> GetAllAsync();
    Task<List<StudentDocumentDto>> GetByStudentIdAsync(int studentId);
    Task<StudentDocumentDto> CreateAsync(StudentDocumentDto dto);
    Task DeleteAsync(int id);
}

