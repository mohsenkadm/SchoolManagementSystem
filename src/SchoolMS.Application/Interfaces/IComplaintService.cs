using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IComplaintService
{
    Task<List<ComplaintDto>> GetAllAsync();
    Task<ComplaintDto?> GetByIdAsync(int id);
    Task<ComplaintDto> CreateAsync(ComplaintDto dto);
    Task<ComplaintDto> UpdateAsync(ComplaintDto dto);
    Task DeleteAsync(int id);
    Task ResolveAsync(int id, string resolution);
}

