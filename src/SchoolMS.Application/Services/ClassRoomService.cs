using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ClassRoomService : IClassRoomService
{
    private readonly IRepository<ClassRoom> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClassRoomService(IRepository<ClassRoom> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<ClassRoomDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(c => c.Grade).Include(c => c.Division).Include(c => c.AcademicYear).Include(c => c.Branch)
            .ToListAsync();
        return _mapper.Map<List<ClassRoomDto>>(items);
    }
    public async Task<List<ClassRoomDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _repository.Query().Where(c => c.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(c => c.BranchId == branchId.Value);
        var items = await query
            .Include(c => c.Grade).Include(c => c.Division).Include(c => c.AcademicYear).Include(c => c.Branch)
            .ToListAsync();
        return _mapper.Map<List<ClassRoomDto>>(items);
    }
    public async Task<ClassRoomDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query().Include(c => c.Grade).Include(c => c.Division)
            .Include(c => c.AcademicYear).Include(c => c.Branch).FirstOrDefaultAsync(c => c.Id == id);
        return entity == null ? null : _mapper.Map<ClassRoomDto>(entity);
    }
    public async Task<ClassRoomDto> CreateAsync(ClassRoomDto dto)
    { var e = _mapper.Map<ClassRoom>(dto); e.Id = 0; await _repository.AddAsync(e); await _unitOfWork.SaveChangesAsync(); return _mapper.Map<ClassRoomDto>(e); }
    public async Task<ClassRoomDto> UpdateAsync(ClassRoomDto dto)
    {
        var e = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        e.GradeId = dto.GradeId; e.DivisionId = dto.DivisionId; e.AcademicYearId = dto.AcademicYearId; e.BranchId = dto.BranchId;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ClassRoomDto>(e);
    }
    public async Task DeleteAsync(int id)
    { var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException(); e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; _repository.Update(e); await _unitOfWork.SaveChangesAsync(); }
}

