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
        await Task.CompletedTask;
    }

    public async Task RemoveUsedStorageAsync(int schoolId, long fileSizeBytes)
    {
        await Task.CompletedTask;
    }

    // ===== Storage Plans =====

    public async Task<List<StoragePlanDto>> GetActiveStoragePlansAsync()
    {
        return await _context.StoragePlans.IgnoreQueryFilters()
            .Where(p => !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.StorageGB)
            .Select(p => new StoragePlanDto
            {
                Id = p.Id, PlanName = p.PlanName, StorageGB = p.StorageGB,
                Price = p.Price, Description = p.Description, IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<StoragePlanDto>> GetAllStoragePlansAsync()
    {
        return await _context.StoragePlans.IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.StorageGB)
            .Select(p => new StoragePlanDto
            {
                Id = p.Id, PlanName = p.PlanName, StorageGB = p.StorageGB,
                Price = p.Price, Description = p.Description, IsActive = p.IsActive,
                RequestCount = p.StorageRequests.Count(r => !r.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<StoragePlanDto> CreateStoragePlanAsync(StoragePlanDto dto)
    {
        var plan = new StoragePlan
        {
            PlanName = dto.PlanName, StorageGB = dto.StorageGB, Price = dto.Price,
            Description = dto.Description, IsActive = dto.IsActive,
            SchoolId = 1, CreatedAt = DateTime.UtcNow, CreatedBy = "Platform"
        };
        _context.StoragePlans.Add(plan);
        await _context.SaveChangesAsync();
        dto.Id = plan.Id;
        return dto;
    }

    public async Task<StoragePlanDto> UpdateStoragePlanAsync(StoragePlanDto dto)
    {
        var plan = await _context.StoragePlans.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == dto.Id && !p.IsDeleted)
            ?? throw new KeyNotFoundException("Storage plan not found.");

        plan.PlanName = dto.PlanName;
        plan.StorageGB = dto.StorageGB;
        plan.Price = dto.Price;
        plan.Description = dto.Description;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;
        plan.UpdatedBy = "Platform";
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task DeleteStoragePlanAsync(int planId)
    {
        var plan = await _context.StoragePlans.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == planId && !p.IsDeleted)
            ?? throw new KeyNotFoundException("Storage plan not found.");

        plan.IsDeleted = true;
        plan.DeletedAt = DateTime.UtcNow;
        plan.DeletedBy = "Platform";
        await _context.SaveChangesAsync();
    }

    // ===== Storage Requests =====

    public async Task<StorageRequestDto> RequestExtraStorageAsync(int schoolId, int storagePlanId, string? notes)
    {
        var sub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No active subscription found.");

        var storagePlan = await _context.StoragePlans.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == storagePlanId && !p.IsDeleted && p.IsActive)
            ?? throw new InvalidOperationException("Storage plan not found or inactive.");

        var request = new StorageRequest
        {
            SchoolSubscriptionId = sub.Id,
            StoragePlanId = storagePlan.Id,
            RequestedGB = storagePlan.StorageGB,
            PricePerGB = storagePlan.StorageGB > 0 ? Math.Round(storagePlan.Price / storagePlan.StorageGB, 2) : 0,
            TotalPrice = storagePlan.Price,
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
            StoragePlanId = storagePlan.Id,
            StoragePlanName = storagePlan.PlanName,
            SchoolId = schoolId,
            RequestedGB = request.RequestedGB,
            PricePerGB = request.PricePerGB,
            TotalPrice = request.TotalPrice,
            Notes = request.Notes,
            CreatedAt = request.CreatedAt
        };
    }

    public async Task<List<StorageRequestDto>> GetSchoolRequestsAsync(int schoolId)
    {
        return await _context.StorageRequests.IgnoreQueryFilters()
            .Where(r => !r.IsDeleted && r.SchoolId == schoolId)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.SystemSubscriptionPlan)
            .Include(r => r.StoragePlan)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new StorageRequestDto
            {
                Id = r.Id,
                SchoolSubscriptionId = r.SchoolSubscriptionId,
                StoragePlanId = r.StoragePlanId,
                StoragePlanName = r.StoragePlan != null ? r.StoragePlan.PlanName : null,
                SchoolId = r.SchoolId,
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

    public async Task<List<StorageRequestDto>> GetPendingRequestsAsync()
    {
        return await _context.StorageRequests.IgnoreQueryFilters()
            .Where(r => !r.IsProcessed && !r.IsDeleted)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.School)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.SystemSubscriptionPlan)
            .Include(r => r.StoragePlan)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new StorageRequestDto
            {
                Id = r.Id,
                SchoolSubscriptionId = r.SchoolSubscriptionId,
                StoragePlanId = r.StoragePlanId,
                StoragePlanName = r.StoragePlan != null ? r.StoragePlan.PlanName : null,
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

    public async Task<List<StorageRequestDto>> GetAllRequestsAsync()
    {
        return await _context.StorageRequests.IgnoreQueryFilters()
            .Where(r => !r.IsDeleted)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.School)
            .Include(r => r.SchoolSubscription).ThenInclude(s => s.SystemSubscriptionPlan)
            .Include(r => r.StoragePlan)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new StorageRequestDto
            {
                Id = r.Id,
                SchoolSubscriptionId = r.SchoolSubscriptionId,
                StoragePlanId = r.StoragePlanId,
                StoragePlanName = r.StoragePlan != null ? r.StoragePlan.PlanName : null,
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
