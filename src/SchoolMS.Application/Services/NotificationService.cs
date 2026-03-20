using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IRepository<Notification> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<NotificationDto>> GetAllAsync()
        => _mapper.Map<List<NotificationDto>>(await _repository.Query().OrderByDescending(n => n.CreatedAt).ToListAsync());

    public async Task<List<NotificationDto>> GetBySchoolIdAsync(int schoolId)
        => _mapper.Map<List<NotificationDto>>(await _repository.Query()
            .Where(n => n.SchoolId == schoolId)
            .OrderByDescending(n => n.CreatedAt).ToListAsync());

    public async Task<NotificationDto> CreateAsync(NotificationDto dto)
    {
        var entity = _mapper.Map<Notification>(dto); entity.Id = 0;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<NotificationDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAsSentAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsSent = true; entity.SentAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }
}

