using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ParentService : IParentService
{
    private readonly IRepository<Parent> _repository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ParentService(IRepository<Parent> repository, IRepository<Student> studentRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ParentDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(p => p.Children.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Branch)
            .Include(p => p.School)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<ParentDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(p => p.SchoolId == schoolId)
            .Include(p => p.Children.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Branch)
            .Include(p => p.School)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<ParentDto?> GetByIdAsync(int id)
    {
        var p = await _repository.Query()
            .Include(x => x.Children.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Branch)
            .Include(x => x.School)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return null;
        return MapToDto(p);
    }

    public async Task<ParentDto> CreateAsync(ParentDto dto)
    {
        var entity = new Parent
        {
            FatherName = dto.FatherName, FatherPhone = dto.FatherPhone, FatherEmail = dto.FatherEmail,
            FatherOccupation = dto.FatherOccupation, MotherName = dto.MotherName, MotherPhone = dto.MotherPhone,
            MotherEmail = dto.MotherEmail, MotherOccupation = dto.MotherOccupation, GuardianName = dto.GuardianName,
            GuardianPhone = dto.GuardianPhone, GuardianRelation = dto.GuardianRelation, Address = dto.Address,
            ProfileImage = dto.ProfileImage,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // Link selected students
        if (dto.StudentIds.Count > 0)
        {
            foreach (var sid in dto.StudentIds)
            {
                var student = await _studentRepository.GetByIdAsync(sid);
                if (student != null) { student.ParentId = entity.Id; _studentRepository.Update(student); }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        dto.Id = entity.Id;
        return dto;
    }

    public async Task<ParentDto> UpdateAsync(ParentDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.FatherName = dto.FatherName; entity.FatherPhone = dto.FatherPhone; entity.FatherEmail = dto.FatherEmail;
        entity.FatherOccupation = dto.FatherOccupation; entity.MotherName = dto.MotherName; entity.MotherPhone = dto.MotherPhone;
        entity.MotherEmail = dto.MotherEmail; entity.MotherOccupation = dto.MotherOccupation; entity.GuardianName = dto.GuardianName;
        entity.GuardianPhone = dto.GuardianPhone; entity.GuardianRelation = dto.GuardianRelation; entity.Address = dto.Address;
        if (!string.IsNullOrEmpty(dto.ProfileImage))
            entity.ProfileImage = dto.ProfileImage;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        // Re-link students
        var currentChildren = await _studentRepository.Query()
            .Where(s => s.ParentId == entity.Id && !s.IsDeleted).ToListAsync();
        foreach (var child in currentChildren)
        {
            if (!dto.StudentIds.Contains(child.Id)) { child.ParentId = null; _studentRepository.Update(child); }
        }
        foreach (var sid in dto.StudentIds)
        {
            if (!currentChildren.Any(c => c.Id == sid))
            {
                var student = await _studentRepository.GetByIdAsync(sid);
                if (student != null) { student.ParentId = entity.Id; _studentRepository.Update(student); }
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    private static ParentDto MapToDto(Parent p)
    {
        var activeChildren = p.Children.Where(c => !c.IsDeleted).ToList();
        return new ParentDto
        {
            Id = p.Id, FatherName = p.FatherName, FatherPhone = p.FatherPhone, FatherEmail = p.FatherEmail,
            FatherOccupation = p.FatherOccupation, MotherName = p.MotherName, MotherPhone = p.MotherPhone,
            MotherEmail = p.MotherEmail, MotherOccupation = p.MotherOccupation, GuardianName = p.GuardianName,
            GuardianPhone = p.GuardianPhone, GuardianRelation = p.GuardianRelation, Address = p.Address,
            ProfileImage = p.ProfileImage,
            ChildrenCount = activeChildren.Count,
            SchoolId = p.SchoolId,
            SchoolName = p.School?.Name,
            BranchName = string.Join(", ", activeChildren.Select(c => c.Branch?.Name).Where(n => n != null).Distinct()),
            Students = activeChildren.Select(c => new ParentStudentInfo { Id = c.Id, FullName = c.FullName, BranchName = c.Branch?.Name }).ToList(),
            StudentIds = activeChildren.Select(c => c.Id).ToList()
        };
    }
}

