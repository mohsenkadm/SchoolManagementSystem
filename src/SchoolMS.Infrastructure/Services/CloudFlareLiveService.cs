using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.Infrastructure.Services;

public class CloudFlareLiveService : ICloudFlareLiveService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CloudFlareLiveService> _logger;

    public CloudFlareLiveService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CloudFlareLiveService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CloudflareResponseDto?> CreateLiveInputAsync(string liveInputName)
    {
        var settings = _configuration.GetSection("CloudflareLive");
        var accessToken = settings["AccessToken"]!;
        var accountId = settings["AccountId"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var body = JsonSerializer.Serialize(new
        {
            meta = new { name = liveInputName },
            recording = new { mode = "automatic" },
            deleteRecordingAfterDays = 30
        });

        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(
            $"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream/live_inputs", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<CloudflareResponseDto>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _logger.LogInformation("Cloudflare live input created: {Uid}", result?.Result?.Uid);
            return result;
        }

        _logger.LogWarning("Failed to create Cloudflare live input: {Response}", responseBody);
        return JsonSerializer.Deserialize<CloudflareResponseDto>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> DeleteLiveInputAsync(string uid)
    {
        var settings = _configuration.GetSection("CloudflareLive");
        var accessToken = settings["AccessToken"]!;
        var accountId = settings["AccountId"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var response = await client.DeleteAsync(
            $"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream/live_inputs/{uid}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Cloudflare live input deleted: {Uid}", uid);
            return true;
        }

        _logger.LogWarning("Failed to delete Cloudflare live input {Uid}: {Status}", uid, response.StatusCode);
        return false;
    }
}
