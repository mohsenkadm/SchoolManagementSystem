using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolMS.Application.Interfaces;
using SchoolMS.Application.Settings;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class OneSignalNotificationService : IOneSignalNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly OneSignalSettings _settings;
    private readonly IRepository<School> _schoolRepository;
    private readonly ILogger<OneSignalNotificationService> _logger;

    public OneSignalNotificationService(HttpClient httpClient, IOptions<OneSignalSettings> settings,
        IRepository<School> schoolRepository,
        ILogger<OneSignalNotificationService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _schoolRepository = schoolRepository;
        _logger = logger;
    }

    public async Task SendToPersonTypesAsync(string title, string message, IEnumerable<string> personTypes, int schoolId)
    {
        var filters = BuildSchoolFilter(schoolId);
        filters.AddRange(BuildPersonTypeFilters(personTypes));
        await SendAsync(title, message, filters, schoolId);
    }

    public async Task SendToClassRoomAsync(string title, string message, IEnumerable<string> personTypes,
        int schoolId, int classRoomId)
    {
        var filters = BuildSchoolFilter(schoolId);
        filters.Add(new Dictionary<string, object> { ["field"] = "tag", ["key"] = "classRoomId", ["relation"] = "=", ["value"] = classRoomId.ToString() });
        filters.AddRange(BuildPersonTypeFilters(personTypes));
        await SendAsync(title, message, filters, schoolId);
    }

    public async Task SendToIndividualAsync(string title, string message, int personId, string personType, int schoolId)
    {
        var filters = BuildSchoolFilter(schoolId);
        filters.Add(new Dictionary<string, object> { ["field"] = "tag", ["key"] = "personId", ["relation"] = "=", ["value"] = personId.ToString() });
        filters.Add(new Dictionary<string, object> { ["field"] = "tag", ["key"] = "personType", ["relation"] = "=", ["value"] = personType });
        await SendAsync(title, message, filters, schoolId);
    }

    public async Task SendToSchoolAsync(string title, string message, int schoolId)
    {
        var filters = BuildSchoolFilter(schoolId);
        await SendAsync(title, message, filters, schoolId);
    }

    private static List<Dictionary<string, object>> BuildSchoolFilter(int schoolId)
    {
        return new List<Dictionary<string, object>>
        {
            new() { ["field"] = "tag", ["key"] = "schoolId", ["relation"] = "=", ["value"] = schoolId.ToString() }
        };
    }

    private static List<Dictionary<string, object>> BuildPersonTypeFilters(IEnumerable<string> personTypes)
    {
        var filters = new List<Dictionary<string, object>>();
        var types = personTypes.ToList();
        for (int i = 0; i < types.Count; i++)
        {
            if (i > 0)
                filters.Add(new Dictionary<string, object> { ["operator"] = "OR" });

            filters.Add(new Dictionary<string, object>
            {
                ["field"] = "tag", ["key"] = "personType", ["relation"] = "=", ["value"] = types[i]
            });
        }
        return filters;
    }

    private async Task SendAsync(string title, string message, List<Dictionary<string, object>> filters, int schoolId)
    {
        // Try school-specific OneSignal credentials first, fall back to global settings
        string? appId = null;
        string? apiKey = null;

        if (schoolId > 0)
        {
            var school = await _schoolRepository.Query()
                .IgnoreQueryFilters()
                .Where(s => s.Id == schoolId && !s.IsDeleted)
                .Select(s => new { s.OneSignalAppId, s.OneSignalApiKey })
                .FirstOrDefaultAsync();

            if (school != null && !string.IsNullOrWhiteSpace(school.OneSignalAppId) && !string.IsNullOrWhiteSpace(school.OneSignalApiKey))
            {
                appId = school.OneSignalAppId;
                apiKey = school.OneSignalApiKey;
            }
        }

        // Fall back to global settings
        appId ??= _settings.AppId;
        apiKey ??= _settings.RestApiKey;

        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OneSignal is not configured for school {SchoolId} and no global fallback. Skipping notification: {Title}", schoolId, title);
            return;
        }

        try
        {
            var payload = new
            {
                app_id = appId,
                headings = new { en = title },
                contents = new { en = message },
                filters
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {apiKey}");

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("OneSignal API error {StatusCode}: {Body}", (int)response.StatusCode, body);
            else
                _logger.LogInformation("OneSignal notification sent: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OneSignal notification: {Title}", title);
        }
    }
}
