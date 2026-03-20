using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IRepository<AuditLog> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogService(IRepository<AuditLog> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<DataTableResponse<AuditLogDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _repository.Query().AsQueryable();
        var total = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(request.SearchValue))
        {
            var search = request.SearchValue.ToLower();
            query = query.Where(a => (a.UserName != null && a.UserName.ToLower().Contains(search))
                || a.Action.ToLower().Contains(search)
                || (a.EntityName != null && a.EntityName.ToLower().Contains(search)));
        }

        var filtered = await query.CountAsync();
        var data = await query.OrderByDescending(a => a.Timestamp).Skip(request.Start).Take(request.Length).ToListAsync();

        return new DataTableResponse<AuditLogDto>
        {
            Draw = request.Draw, RecordsTotal = total, RecordsFiltered = filtered,
            Data = data.Select(a => new AuditLogDto
            {
                Id = a.Id, UserName = a.UserName, Action = a.Action, EntityName = a.EntityName,
                EntityId = a.EntityId, OldValues = a.OldValues, NewValues = a.NewValues,
                IpAddress = a.IpAddress, Timestamp = a.Timestamp, PageName = a.PageName
            }).ToList()
        };
    }

    public async Task LogAsync(string userId, string userName, string action, string? entityName, int? entityId,
        string? oldValues, string? newValues, string? ipAddress, string? pageName)
    {
        var log = new AuditLog
        {
            UserId = userId, UserName = userName, Action = action, EntityName = entityName, EntityId = entityId,
            OldValues = oldValues, NewValues = newValues, IpAddress = ipAddress, Timestamp = DateTime.UtcNow,
            PageName = pageName
        };
        await _repository.AddAsync(log); await _unitOfWork.SaveChangesAsync();
    }
}

