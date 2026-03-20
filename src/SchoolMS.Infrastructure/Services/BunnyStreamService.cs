using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.Infrastructure.Services;

public class BunnyStreamService : IBunnyStreamService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BunnyStreamService> _logger;

    public BunnyStreamService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BunnyStreamService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<VideoUploadResultDto> UploadVideoAsync(Stream videoStream, string fileName)
    {
        var settings = _configuration.GetSection("BunnyStream");
        var apiKey = settings["ApiKey"]!;
        var libraryId = settings["LibraryId"]!;
        var cdnHostname = settings["CdnHostname"]!;

        string[] videoExtensions = [".mp4", ".mov", ".avi", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg"];
        var fileExtension = Path.GetExtension(fileName).ToLower();
        if (!videoExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException("Invalid video file format. Supported: mp4, mov, avi, mkv, flv, wmv, webm, mpeg, mpg");
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("AccessKey", apiKey);

        // Step 1: Create video entry
        var videoTitle = $"{Guid.NewGuid()}";
        var createContent = new StringContent(
            JsonSerializer.Serialize(new { title = videoTitle }),
            Encoding.UTF8, "application/json");

        var createResponse = await client.PostAsync(
            $"https://video.bunnycdn.com/library/{libraryId}/videos", createContent);
        createResponse.EnsureSuccessStatusCode();

        var createBody = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<BunnyStreamResponseDto>(createBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Step 2: Upload video content
        using var streamContent = new StreamContent(videoStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var uploadResponse = await client.PutAsync(
            $"https://video.bunnycdn.com/library/{libraryId}/videos/{createResult.guid}", streamContent);
        uploadResponse.EnsureSuccessStatusCode();

        // Step 3: Wait for encoding
        await WaitForEncodingAsync(client, libraryId, createResult.guid);

        // Step 4: Build result URLs
        var baseUrl = $"https://{cdnHostname}/{createResult.guid}";

        var resolutions = new List<VideoResolutionDto>
        {
            new() { Resolution = "auto", Url = $"{baseUrl}/playlist.m3u8", Width = 0, Height = 0 },
            new() { Resolution = "480p", Url = $"{baseUrl}/playlist_480p.m3u8", Width = 854, Height = 480 },
            new() { Resolution = "720p", Url = $"{baseUrl}/playlist_720p.m3u8", Width = 1280, Height = 720 },
            new() { Resolution = "1080p", Url = $"{baseUrl}/playlist_1080p.m3u8", Width = 1920, Height = 1080 }
        };

        return new VideoUploadResultDto
        {
            VideoId = createResult.guid,
            ThumbnailUrl = $"{baseUrl}/thumbnail.jpg",
            MasterPlaylistUrl = $"{baseUrl}/playlist.m3u8",
            AvailableResolutions = resolutions
        };
    }

    public async Task DeleteVideoAsync(string videoId)
    {
        var settings = _configuration.GetSection("BunnyStream");
        var apiKey = settings["ApiKey"]!;
        var libraryId = settings["LibraryId"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("AccessKey", apiKey);

        var response = await client.DeleteAsync(
            $"https://video.bunnycdn.com/library/{libraryId}/videos/{videoId}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to delete Bunny Stream video {VideoId}: {Status}", videoId, response.StatusCode);
        }
    }

    private async Task WaitForEncodingAsync(HttpClient client, string libraryId, string videoGuid, int maxAttempts = 30)
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            await Task.Delay(2000);

            var response = await client.GetAsync(
                $"https://video.bunnycdn.com/library/{libraryId}/videos/{videoGuid}");

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var info = JsonSerializer.Deserialize<BunnyStreamResponseDto>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Status: 3=Finished, 4=Resolution Finished
                if (info?.status is "3" or "4")
                    return;
            }
        }

        _logger.LogWarning("Video encoding timeout for {VideoGuid}", videoGuid);
    }
}
