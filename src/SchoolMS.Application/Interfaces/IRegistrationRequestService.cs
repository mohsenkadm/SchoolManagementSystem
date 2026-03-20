using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IRegistrationRequestService
{
    Task<List<SchoolRegistrationRequestDto>> GetAllAsync();
    Task<SchoolRegistrationRequestDto?> GetByIdAsync(int id);
    Task<SchoolRegistrationRequestDto> CreateAsync(CreateRegistrationRequestDto dto, string? ipAddress);
    Task UpdateStatusAsync(UpdateRequestStatusDto dto);
    Task DeleteAsync(int id);
    Task<RegistrationRequestStatsDto> GetStatsAsync();
}

public class RegistrationRequestStatsDto
{
    public int TotalRequests { get; set; }
    public int NewRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public double ConversionRate { get; set; }
}
