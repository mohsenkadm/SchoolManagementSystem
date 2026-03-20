using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface ISalaryService
{
    Task<List<SalarySetupDto>> GetAllSetupsAsync();
    Task<List<SalarySetupDto>> GetBySchoolIdAsync(int schoolId);
    Task<SalarySetupDto?> GetByPersonAsync(int personId, PersonType personType, int schoolId);
    Task<SalarySetupDto?> GetSetupByIdAsync(int id);
    Task<SalarySetupDto> CreateSetupAsync(SalarySetupDto dto);
    Task<SalarySetupDto> UpdateSetupAsync(SalarySetupDto dto);
    Task DeleteSetupAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}

