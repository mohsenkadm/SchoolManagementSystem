using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetAllAsync();
    Task<List<NotificationDto>> GetBySchoolIdAsync(int schoolId);
    Task<NotificationDto> CreateAsync(NotificationDto dto);
    Task DeleteAsync(int id);
    Task MarkAsSentAsync(int id);
}

