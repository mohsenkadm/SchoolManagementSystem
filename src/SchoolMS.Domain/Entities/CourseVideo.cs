namespace SchoolMS.Domain.Entities;

public class CourseVideo : BaseEntity
{
    public int CourseId { get; set; }
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

    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<VideoComment> Comments { get; set; } = new List<VideoComment>();
    public virtual ICollection<VideoRating> Ratings { get; set; } = new List<VideoRating>();
    public virtual ICollection<VideoFavorite> Favorites { get; set; } = new List<VideoFavorite>();
    public virtual ICollection<VideoLike> Likes { get; set; } = new List<VideoLike>();
    public virtual ICollection<VideoSeen> Seens { get; set; } = new List<VideoSeen>();
}
