using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class CarouselService : ICarouselService
{
    private readonly IRepository<CarouselImage> _repository;
    private readonly IRepository<Branch> _branchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CarouselService(IRepository<CarouselImage> repository, IRepository<Branch> branchRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CarouselImageDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(c => c.Branch)
            .Include(c => c.School)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return items.Select(c => new CarouselImageDto
        {
            Id = c.Id, ImageUrl = c.ImageUrl, SortOrder = c.SortOrder,
            IsActive = c.IsActive, BranchId = c.BranchId,
            BranchName = c.Branch?.Name,
            SchoolId = c.SchoolId,
            SchoolName = c.School?.Name
        }).ToList();
    }

    public async Task<List<CarouselImageDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _repository.Query().Where(c => c.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(c => c.BranchId == branchId.Value);
        var items = await query
            .Include(c => c.Branch)
            .Include(c => c.School)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return items.Select(c => new CarouselImageDto
        {
            Id = c.Id, ImageUrl = c.ImageUrl, SortOrder = c.SortOrder,
            IsActive = c.IsActive, BranchId = c.BranchId,
            BranchName = c.Branch?.Name,
            SchoolId = c.SchoolId,
            SchoolName = c.School?.Name
        }).ToList();
    }

    public async Task<CarouselImageDto> CreateAsync(CarouselImageDto dto)
    {
        // Resolve SchoolId from the branch if available
        int schoolId = dto.SchoolId;
        if (dto.BranchId.HasValue && dto.BranchId.Value > 0)
        {
            var branch = await _branchRepository.GetByIdAsync(dto.BranchId.Value);
            if (branch != null && schoolId == 0)
                schoolId = branch.SchoolId;
        }

        var entity = new CarouselImage
        {
            ImageUrl = dto.ImageUrl ?? string.Empty,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive,
            BranchId = dto.BranchId.HasValue && dto.BranchId.Value > 0 ? dto.BranchId : null,
            SchoolId = schoolId
        };
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<CarouselImageDto> UpdateAsync(CarouselImageDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();

        entity.BranchId = dto.BranchId.HasValue && dto.BranchId.Value > 0 ? dto.BranchId : null;
        if (dto.SchoolId > 0)
            entity.SchoolId = dto.SchoolId;

        entity.ImageUrl = dto.ImageUrl ?? entity.ImageUrl;
        entity.SortOrder = dto.SortOrder;
        entity.IsActive = dto.IsActive;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderAsync(List<int> ids)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            var entity = await _repository.GetByIdAsync(ids[i]);
            if (entity != null) { entity.SortOrder = i; _repository.Update(entity); }
        }
        await _unitOfWork.SaveChangesAsync();
    }
}

