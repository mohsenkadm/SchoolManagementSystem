using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ITeacherService
{
    Task<List<TeacherDto>> GetAllAsync();
    Task<List<TeacherDto>> GetBySchoolIdAsync(int schoolId);
    Task<TeacherDto?> GetByIdAsync(int id);
    Task<DataTableResponse<TeacherDto>> GetDataTableAsync(DataTableRequest request);
    Task<TeacherDto> CreateAsync(CreateTeacherDto dto);
    Task<TeacherDto> UpdateAsync(UpdateTeacherDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}
