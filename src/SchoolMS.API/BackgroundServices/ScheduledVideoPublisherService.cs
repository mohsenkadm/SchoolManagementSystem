using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;

namespace SchoolMS.API.BackgroundServices;

/// <summary>
/// Background service that checks every minute for scheduled videos whose ScheduledPublishAt has passed.
/// When found, it sets IsScheduled = false and sends push notifications to subscribed students.
/// </summary>
public class ScheduledVideoPublisherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledVideoPublisherService> _logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    public ScheduledVideoPublisherService(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduledVideoPublisherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduledVideoPublisherService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledVideosAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledVideoPublisherService.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task ProcessScheduledVideosAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var videoRepo = scope.ServiceProvider.GetRequiredService<IRepository<CourseVideo>>();
        var courseRepo = scope.ServiceProvider.GetRequiredService<IRepository<Course>>();
        var subscriptionRepo = scope.ServiceProvider.GetRequiredService<IRepository<StudentSubscription>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var pushService = scope.ServiceProvider.GetRequiredService<IOneSignalNotificationService>();

        var now = DateTime.UtcNow;

        // Find all scheduled videos that are due for publishing
        var dueVideos = await videoRepo.Query()
            .Where(v => v.IsScheduled && v.ScheduledPublishAt.HasValue && v.ScheduledPublishAt.Value <= now)
            .Include(v => v.Course)
                .ThenInclude(c => c.Subject)
            .ToListAsync(ct);

        if (dueVideos.Count == 0) return;

        _logger.LogInformation("Found {Count} scheduled videos to publish.", dueVideos.Count);

        foreach (var video in dueVideos)
        {
            video.IsScheduled = false;
            videoRepo.Update(video);

            // Find all students with active subscriptions to this course's subject
            var subjectId = video.Course.SubjectId;
            var schoolId = video.Course.SchoolId;

            var subscribedStudentIds = await subscriptionRepo.Query()
                .Where(s => s.Status == SubscriptionStatus.Approved
                    && s.EndDate >= now
                    && s.OnlineSubscriptionPlan.SubjectId == subjectId)
                .Include(s => s.OnlineSubscriptionPlan)
                .Select(s => s.StudentId)
                .Distinct()
                .ToListAsync(ct);

            if (subscribedStudentIds.Count > 0)
            {
                var title = "New Video Available 🎬";
                var message = $"\"{video.Title}\" is now available in {video.Course.Subject?.SubjectName ?? video.Course.Title}. Watch it now!";

                // Send to all subscribed students of this school
                try
                {
                    await pushService.SendToPersonTypesAsync(title, message, new[] { "Student" }, schoolId);
                    _logger.LogInformation("Sent notification for video '{Title}' to {Count} subscribers.", video.Title, subscribedStudentIds.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send push notification for video '{Title}'.", video.Title);
                }
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Published {Count} scheduled videos.", dueVideos.Count);
    }
}
