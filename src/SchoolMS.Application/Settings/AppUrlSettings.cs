namespace SchoolMS.Application.Settings;

/// <summary>
/// Centralized base URL configuration for the entire solution.
/// All file/image/document paths are stored with the full URL
/// so they can be opened from Web, API, or Portal projects.
/// </summary>
public static class AppUrlSettings
{
    /// <summary>
    /// Base URL for the Web project (e.g., https://localhost:7060 or https://yourdomain.com).
    /// Files saved via the Web project are prefixed with this URL.
    /// </summary>
    public static string WebBaseUrl { get; set; } = "https://localhost:7060";

    /// <summary>
    /// Base URL for the API project (e.g., https://localhost:7246 or https://api.yourdomain.com).
    /// Files saved via the API project are prefixed with this URL.
    /// </summary>
    public static string ApiBaseUrl { get; set; } = "https://localhost:7246";

    /// <summary>
    /// Builds the full URL for a file path saved via the Web project.
    /// </summary>
    public static string BuildWebUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        if (relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return relativePath;
        return $"{WebBaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    /// <summary>
    /// Builds the full URL for a file path saved via the API project.
    /// </summary>
    public static string BuildApiUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        if (relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return relativePath;
        return $"{ApiBaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }
}
