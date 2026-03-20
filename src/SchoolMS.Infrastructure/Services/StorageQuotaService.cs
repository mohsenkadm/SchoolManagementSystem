using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class StorageQuotaService : IStorageQuotaService
{
    private readonly SchoolDbContext _context;

    public StorageQuotaService(SchoolDbContext context) => _context = context;

    public async Task<StorageQuotaDto?> GetQuotaAsync(int schoolId)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync();

        if (sub == null) return null;

        var usedBytes = await _context.CourseVideos.IgnoreQueryFilters()
            .Where(v => v.SchoolId == schoolId && !v.IsDeleted)
            .SumAsync(v => v.FileSizeBytes);

        return new StorageQuotaDto
        {
            StorageLimitGB = sub.SystemSubscriptionPlan.StorageLimitGB,
            ExtraStorageGB = sub.ExtraStorageGB,
            UsedStorageGB = Math.Round((decimal)usedBytes / (1024m * 1024m * 1024m), 2)
        };
    }

    public async Task<(bool allowed, string? error)> CanUploadAsync(int schoolId, long fileSizeBytes)
    {
        var quota = await GetQuotaAsync(schoolId);
        if (quota == null)
            return (false, "No active subscription found.");

        if (!quota.HasSpace(fileSizeBytes))
        {
            var fileSizeGB = Math.Round((decimal)fileSizeBytes / (1024m * 1024m * 1024m), 2);
            return (false, $"Not enough storage. Available: {quota.AvailableGB:F2} GB, File size: {fileSizeGB:F2} GB. Please request additional storage.");
        }

        return (true, null);
    }

    public async Task AddUsedStorageAsync(int schoolId, long fileSizeBytes)
    {
        // FileSizeBytes is tracked on individual CourseVideo entities, no separate tracking needed
        await Task.CompletedTask;
    }

    public async Task RemoveUsedStorageAsync(int schoolId, long fileSizeBytes)
    {
        await Task.CompletedTask;
    }

    public async Task<StorageRequestDto> RequestExtraStorageAsync(int schoolId, decimal requestedGB, string? notes)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No active subscription found.");

        var request = new StorageRequest
        {
            SchoolSubscriptionId = sub.Id,
            RequestedGB = requestedGB,
            PricePerGB = 1.0m, // default price per GB — can be configured
            TotalPrice = requestedGB * 1.0m,
            Notes = notes,
            SchoolId = schoolId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "School"
        };

        _context.StorageRequests.Add(request);
        await _context.SaveChangesAsync();

        return new StorageRequestDto
        {
            Id = request.Id,
            SchoolSubscriptionId = request.SchoolSubscriptionId,
            SchoolId = schoolId,
            RequestedGB = request.RequestedGB,
            PricePerGB = request.PricePerGB,
            TotalPrice = request.TotalPrice,
            Notes = request.Notes,
            CreatedAt = request.CreatedAt
        };
    }

    public async Task<List<StorageRequestDto>> GetPendingRequestsAsync()
    {
        return await _context.StorageRequests.IgnoreQueryFilters()
            .Where(r => !r.IsProcessed && !r.IsDeleted)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.School)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.SystemSubscriptionPlan)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new StorageRequestDto
            {
                Id = r.Id,
                SchoolSubscriptionId = r.SchoolSubscriptionId,
                SchoolId = r.SchoolSubscription.SchoolId,
                SchoolName = r.SchoolSubscription.School.Name,
                PlanName = r.SchoolSubscription.SystemSubscriptionPlan.PlanName,
                RequestedGB = r.RequestedGB,
                PricePerGB = r.PricePerGB,
                TotalPrice = r.TotalPrice,
                Notes = r.Notes,
                IsApproved = r.IsApproved,
                IsProcessed = r.IsProcessed,
                ProcessedAt = r.ProcessedAt,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task ApproveStorageRequestAsync(int requestId, string approvedBy)
    {
        var request = await _context.StorageRequests.IgnoreQueryFilters()
            .Include(r => r.SchoolSubscription)
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted)
            ?? throw new KeyNotFoundException("Storage request not found.");

        request.IsApproved = true;
        request.IsProcessed = true;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedBy = approvedBy;

        request.SchoolSubscription.ExtraStorageGB += request.RequestedGB;
        request.SchoolSubscription.ExtraStoragePrice += request.TotalPrice;

        await _context.SaveChangesAsync();
    }

    public async Task RejectStorageRequestAsync(int requestId, string rejectedBy)
    {
        var request = await _context.StorageRequests.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted)
            ?? throw new KeyNotFoundException("Storage request not found.");

        request.IsApproved = false;
        request.IsProcessed = true;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedBy = rejectedBy;

        await _context.SaveChangesAsync();
    }
}
