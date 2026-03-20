using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IAnnouncementService
{
    Task<List<AnnouncementDto>> GetAllAsync(int? schoolId = null, int? branchId = null);
    Task<AnnouncementDto?> GetByIdAsync(int id);
    Task<AnnouncementDto> CreateAsync(AnnouncementDto dto);
    Task<AnnouncementDto> UpdateAsync(AnnouncementDto dto);
    Task DeleteAsync(int id);
}

