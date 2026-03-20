using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? ThumbnailImage { get; set; }
    public string? BackgroundImage { get; set; }
    public bool IsPublished { get; set; }
    public int VideoCount { get; set; }
    public int LiveStreamCount { get; set; }
    public int SubscriberCount { get; set; }
    public int SchoolId { get; set; }
}

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public string? ThumbnailImage { get; set; }
    public string? BackgroundImage { get; set; }
    public bool IsPublished { get; set; }
    public int SchoolId { get; set; }
}

public class CourseVideoDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BunnyStreamVideoId { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsFreeTrial { get; set; }
    public int ViewCount { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public long FileSizeBytes { get; set; }
    public int SeenCount { get; set; }
    public int LikeCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
}

public class CreateCourseVideoDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsFreeTrial { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
}

public class VideoSortDto
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
}

public class LiveStreamDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? CloudflareStreamId { get; set; }
    public string? StreamUrl { get; set; }
    public string? RtmpsUrl { get; set; }
    public string? StreamKey { get; set; }
    public LiveStreamStatus Status { get; set; }
    public int SeenCount { get; set; }
    public bool IsStandalone => CourseId == null;
}

public class CreateLiveStreamDto
{
    public string Title { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int SchoolId { get; set; }
}

// Bunny Stream
public class VideoUploadResultDto
{
    public string VideoId { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string MasterPlaylistUrl { get; set; } = string.Empty;
    public List<VideoResolutionDto> AvailableResolutions { get; set; } = [];
}

public class VideoResolutionDto
{
    public string Resolution { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}

public class BunnyStreamResponseDto
{
    public string guid { get; set; } = string.Empty;
    public string? status { get; set; }
}

// Cloudflare Live
public class CloudflareResponseDto
{
    public CloudflareResultDto? Result { get; set; }
    public bool Success { get; set; }
    public CloudflareErrorDto[]? Errors { get; set; }
}

public class CloudflareResultDto
{
    public string Uid { get; set; } = string.Empty;
    public CloudflareRtmpsDto? Rtmps { get; set; }
}

public class CloudflareRtmpsDto
{
    public string Url { get; set; } = string.Empty;
    public string StreamKey { get; set; } = string.Empty;
}

public class CloudflareErrorDto
{
    public string Message { get; set; } = string.Empty;
}

public class VideoRateRequestDto
{
    public int StudentId { get; set; }
    public int Rating { get; set; }
}
