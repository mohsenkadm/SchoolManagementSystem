using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly IRepository<LeaveRequest> _repository;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Staff> _staffRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeaveService(IRepository<LeaveRequest> repository, IRepository<Student> studentRepo,
        IRepository<Teacher> teacherRepo, IRepository<Staff> staffRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository; _studentRepo = studentRepo;
        _teacherRepo = teacherRepo; _staffRepo = staffRepo;
        _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<LeaveRequestDto>> GetAllAsync()
    {
        var entities = (await _repository.GetAllAsync()).ToList();
        var dtos = _mapper.Map<List<LeaveRequestDto>>(entities);

        var studentIds = entities.Where(e => e.PersonType == PersonType.Student).Select(e => e.PersonId).Distinct().ToList();
        var teacherIds = entities.Where(e => e.PersonType == PersonType.Teacher).Select(e => e.PersonId).Distinct().ToList();
        var staffIds = entities.Where(e => e.PersonType == PersonType.Staff).Select(e => e.PersonId).Distinct().ToList();

        var studentNames = studentIds.Count > 0
            ? await _studentRepo.Query().Where(s => studentIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();
        var teacherNames = teacherIds.Count > 0
            ? await _teacherRepo.Query().Where(t => teacherIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.FullName)
            : new Dictionary<int, string>();
        var staffNames = staffIds.Count > 0
            ? await _staffRepo.Query().Where(s => staffIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();

        foreach (var dto in dtos)
        {
            dto.PersonName = dto.PersonType switch
            {
                PersonType.Student => studentNames.GetValueOrDefault(dto.PersonId),
                PersonType.Teacher => teacherNames.GetValueOrDefault(dto.PersonId),
                PersonType.Staff => staffNames.GetValueOrDefault(dto.PersonId),
                _ => null
            };
        }
        return dtos;
    }

    public async Task<List<LeaveRequestDto>> GetBySchoolIdAsync(int schoolId)
    {
        var entities = await _repository.Query().Where(l => l.SchoolId == schoolId).ToListAsync();
        var dtos = _mapper.Map<List<LeaveRequestDto>>(entities);
        await ResolvePersonNamesAsync(entities, dtos);
        return dtos;
    }

    public async Task<List<LeaveRequestDto>> GetByPersonAsync(int personId, PersonType personType, int schoolId)
    {
        var entities = await _repository.Query()
            .Where(l => l.PersonId == personId && l.PersonType == personType && l.SchoolId == schoolId)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();
        var dtos = _mapper.Map<List<LeaveRequestDto>>(entities);
        await ResolvePersonNamesAsync(entities, dtos);
        return dtos;
    }

    public async Task<List<LeaveRequestDto>> GetByParentChildrenAsync(int parentId, int schoolId)
    {
        var childrenIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.Id)
            .ToListAsync();
        if (childrenIds.Count == 0) return new List<LeaveRequestDto>();
        var entities = await _repository.Query()
            .Where(l => childrenIds.Contains(l.PersonId) && l.PersonType == PersonType.Student && l.SchoolId == schoolId)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();
        var dtos = _mapper.Map<List<LeaveRequestDto>>(entities);
        await ResolvePersonNamesAsync(entities, dtos);
        return dtos;
    }

    public async Task<LeaveRequestDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<LeaveRequestDto>(entity);
    }

    public async Task<LeaveRequestDto> CreateAsync(LeaveRequestDto dto)
    {
        var entity = _mapper.Map<LeaveRequest>(dto); entity.Id = 0;
        entity.Status = LeaveStatus.Pending;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<LeaveRequestDto>(entity);
    }

    public async Task<LeaveRequestDto> UpdateAsync(LeaveRequestDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.StartDate = dto.StartDate; entity.EndDate = dto.EndDate; entity.Reason = dto.Reason;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<LeaveRequestDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.Status = LeaveStatus.Approved;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.Status = LeaveStatus.Rejected;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    private async Task ResolvePersonNamesAsync(List<LeaveRequest> entities, List<LeaveRequestDto> dtos)
    {
        var studentIds = entities.Where(e => e.PersonType == PersonType.Student).Select(e => e.PersonId).Distinct().ToList();
        var teacherIds = entities.Where(e => e.PersonType == PersonType.Teacher).Select(e => e.PersonId).Distinct().ToList();
        var staffIds = entities.Where(e => e.PersonType == PersonType.Staff).Select(e => e.PersonId).Distinct().ToList();

        var studentNames = studentIds.Count > 0
            ? await _studentRepo.Query().Where(s => studentIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();
        var teacherNames = teacherIds.Count > 0
            ? await _teacherRepo.Query().Where(t => teacherIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.FullName)
            : new Dictionary<int, string>();
        var staffNames = staffIds.Count > 0
            ? await _staffRepo.Query().Where(s => staffIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();

        foreach (var dto in dtos)
        {
            dto.PersonName = dto.PersonType switch
            {
                PersonType.Student => studentNames.GetValueOrDefault(dto.PersonId),
                PersonType.Teacher => teacherNames.GetValueOrDefault(dto.PersonId),
                PersonType.Staff => staffNames.GetValueOrDefault(dto.PersonId),
                _ => null
            };
        }
    }
}

