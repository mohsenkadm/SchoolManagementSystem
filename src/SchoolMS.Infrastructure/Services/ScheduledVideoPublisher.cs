using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class ScheduledVideoPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledVideoPublisher> _logger;

    public ScheduledVideoPublisher(IServiceProvider serviceProvider, ILogger<ScheduledVideoPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

                var now = DateTime.UtcNow;
                var videos = await context.CourseVideos.IgnoreQueryFilters()
                    .Where(v => !v.IsDeleted && v.IsScheduled && v.ScheduledPublishAt != null && v.ScheduledPublishAt <= now)
                    .ToListAsync(stoppingToken);

                if (videos.Count > 0)
                {
                    foreach (var video in videos)
                    {
                        video.IsScheduled = false;
                        _logger.LogInformation("Published scheduled video {VideoId}: {Title}", video.Id, video.Title);
                    }
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Published {Count} scheduled video(s).", videos.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledVideoPublisher");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
