using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllAsync();
    Task<StudentDto?> GetByIdAsync(int id);
    Task<DataTableResponse<StudentDto>> GetDataTableAsync(DataTableRequest request);
    Task<StudentDto> CreateAsync(CreateStudentDto dto);
    Task<StudentDto> UpdateAsync(UpdateStudentDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}
