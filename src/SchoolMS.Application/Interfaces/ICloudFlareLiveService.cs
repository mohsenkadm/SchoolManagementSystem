using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ICloudFlareLiveService
{
    Task<CloudflareResponseDto?> CreateLiveInputAsync(string liveInputName);
    Task<bool> DeleteLiveInputAsync(string uid);
}
