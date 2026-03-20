using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IBunnyStreamService
{
    Task<VideoUploadResultDto> UploadVideoAsync(Stream videoStream, string fileName);
    Task DeleteVideoAsync(string videoId);
}
