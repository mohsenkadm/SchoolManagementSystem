using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrEmployeeService : IHrEmployeeService
{
    private readonly IRepository<HrEmployee> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrEmployeeService(IRepository<HrEmployee> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrEmployeeListDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .Include(e => e.Branch)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return MapEmployeeList(items);
    }

    public async Task<List<HrEmployeeListDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(e => e.SchoolId == schoolId)
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .Include(e => e.Branch)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return MapEmployeeList(items);
    }

    private static List<HrEmployeeListDto> MapEmployeeList(List<HrEmployee> items)
    {

        return items.Select(e => new HrEmployeeListDto
        {
            Id = e.Id,
            EmployeeNumber = e.EmployeeNumber,
            FullName = e.FullName,
            ProfileImage = e.ProfileImage,
            DepartmentName = e.Department?.DepartmentName,
            JobTitleName = e.JobTitle?.TitleName,
            BranchName = e.Branch?.Name,
            Phone = e.Phone,
            Status = e.Status,
            HireDate = e.HireDate,
            EmployeeType = e.EmployeeType,
            Category = e.Category
        }).ToList();
    }

    public async Task<HrEmployeeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .Include(e => e.JobGrade)
            .Include(e => e.Branch)
            .Include(e => e.WorkShift)
            .Include(e => e.DirectManager)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null) return null;

        return new HrEmployeeDto
        {
            Id = entity.Id,
            EmployeeNumber = entity.EmployeeNumber,
            FirstName = entity.FirstName,
            SecondName = entity.SecondName,
            ThirdName = entity.ThirdName,
            LastName = entity.LastName,
            FullName = entity.FullName,
            FullNameAr = entity.FullNameAr,
            DateOfBirth = entity.DateOfBirth,
            Gender = entity.Gender,
            NationalId = entity.NationalId,
            Nationality = entity.Nationality,
            Religion = entity.Religion,
            MaritalStatus = entity.MaritalStatus,
            NumberOfDependents = entity.NumberOfDependents,
            BloodType = entity.BloodType,
            ProfileImage = entity.ProfileImage,
            Phone = entity.Phone,
            Phone2 = entity.Phone2,
            Email = entity.Email,
            PersonalEmail = entity.PersonalEmail,
            Address = entity.Address,
            City = entity.City,
            EmergencyContactName = entity.EmergencyContactName,
            EmergencyContactPhone = entity.EmergencyContactPhone,
            EmergencyContactRelation = entity.EmergencyContactRelation,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.Department?.DepartmentName,
            JobTitleId = entity.JobTitleId,
            JobTitleName = entity.JobTitle?.TitleName,
            JobGradeId = entity.JobGradeId,
            JobGradeName = entity.JobGrade?.GradeName,
            JobGradeStepId = entity.JobGradeStepId,
            BranchId = entity.BranchId,
            BranchName = entity.Branch?.Name,
            WorkShiftId = entity.WorkShiftId,
            WorkShiftName = entity.WorkShift?.ShiftName,
            EmployeeType = entity.EmployeeType,
            Category = entity.Category,
            HireDate = entity.HireDate,
            ProbationEndDate = entity.ProbationEndDate,
            ConfirmationDate = entity.ConfirmationDate,
            Status = entity.Status,
            DirectManagerId = entity.DirectManagerId,
            DirectManagerName = entity.DirectManager?.FullName,
            BadgeCardNumber = entity.BadgeCardNumber,
            FingerprintId = entity.FingerprintId,
            QrCode = entity.QrCode,
            BankName = entity.BankName,
            BankAccountNumber = entity.BankAccountNumber,
            IBAN = entity.IBAN,
            Currency = entity.Currency,
            TaxNumber = entity.TaxNumber,
            SocialSecurityNumber = entity.SocialSecurityNumber,
            HighestQualification = entity.HighestQualification,
            University = entity.University,
            Major = entity.Major,
            GraduationYear = entity.GraduationYear,
            YearsOfExperience = entity.YearsOfExperience,
            Notes = entity.Notes
        };
    }

    public async Task<HrEmployeeDto> CreateAsync(HrEmployeeDto dto)
    {
        var entity = _mapper.Map<HrEmployee>(dto);
        entity.Id = 0;
        entity.EmployeeNumber = await GenerateEmployeeNumberAsync();
        entity.FullName = $"{dto.FirstName} {dto.SecondName} {dto.ThirdName} {dto.LastName}".Replace("  ", " ").Trim();
        entity.Status = HrEmployeeStatus.Active;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEmployeeDto>(entity);
    }

    public async Task<HrEmployeeDto> UpdateAsync(HrEmployeeDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Employee with ID {dto.Id} not found.");

        entity.FirstName = dto.FirstName;
        entity.SecondName = dto.SecondName;
        entity.ThirdName = dto.ThirdName;
        entity.LastName = dto.LastName;
        entity.FullName = $"{dto.FirstName} {dto.SecondName} {dto.ThirdName} {dto.LastName}".Replace("  ", " ").Trim();
        entity.FullNameAr = dto.FullNameAr;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.Gender = dto.Gender;
        entity.NationalId = dto.NationalId;
        entity.Nationality = dto.Nationality;
        entity.Religion = dto.Religion;
        entity.MaritalStatus = dto.MaritalStatus;
        entity.NumberOfDependents = dto.NumberOfDependents;
        entity.BloodType = dto.BloodType;
        entity.ProfileImage = dto.ProfileImage;
        entity.Phone = dto.Phone;
        entity.Phone2 = dto.Phone2;
        entity.Email = dto.Email;
        entity.PersonalEmail = dto.PersonalEmail;
        entity.Address = dto.Address;
        entity.City = dto.City;
        entity.EmergencyContactName = dto.EmergencyContactName;
        entity.EmergencyContactPhone = dto.EmergencyContactPhone;
        entity.EmergencyContactRelation = dto.EmergencyContactRelation;
        entity.DepartmentId = dto.DepartmentId;
        entity.JobTitleId = dto.JobTitleId;
        entity.JobGradeId = dto.JobGradeId;
        entity.JobGradeStepId = dto.JobGradeStepId;
        entity.BranchId = dto.BranchId;
        entity.WorkShiftId = dto.WorkShiftId;
        entity.EmployeeType = dto.EmployeeType;
        entity.Category = dto.Category;
        entity.HireDate = dto.HireDate;
        entity.ProbationEndDate = dto.ProbationEndDate;
        entity.ConfirmationDate = dto.ConfirmationDate;
        entity.Status = dto.Status;
        entity.DirectManagerId = dto.DirectManagerId;
        entity.BadgeCardNumber = dto.BadgeCardNumber;
        entity.FingerprintId = dto.FingerprintId;
        entity.BankName = dto.BankName;
        entity.BankAccountNumber = dto.BankAccountNumber;
        entity.IBAN = dto.IBAN;
        entity.Currency = dto.Currency;
        entity.TaxNumber = dto.TaxNumber;
        entity.SocialSecurityNumber = dto.SocialSecurityNumber;
        entity.HighestQualification = dto.HighestQualification;
        entity.University = dto.University;
        entity.Major = dto.Major;
        entity.GraduationYear = dto.GraduationYear;
        entity.YearsOfExperience = dto.YearsOfExperience;
        entity.Notes = dto.Notes;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEmployeeDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrEmployeeListDto>> GetByDepartmentAsync(int departmentId)
    {
        var items = await _repository.Query()
            .Include(e => e.Department).Include(e => e.JobTitle).Include(e => e.Branch)
            .Where(e => e.DepartmentId == departmentId)
            .ToListAsync();
        return items.Select(e => new HrEmployeeListDto
        {
            Id = e.Id, EmployeeNumber = e.EmployeeNumber, FullName = e.FullName,
            DepartmentName = e.Department?.DepartmentName, JobTitleName = e.JobTitle?.TitleName,
            BranchName = e.Branch?.Name, Phone = e.Phone, Status = e.Status, HireDate = e.HireDate,
            EmployeeType = e.EmployeeType, Category = e.Category
        }).ToList();
    }

    public async Task<List<HrEmployeeListDto>> GetByBranchAsync(int branchId)
    {
        var items = await _repository.Query()
            .Include(e => e.Department).Include(e => e.JobTitle).Include(e => e.Branch)
            .Where(e => e.BranchId == branchId)
            .ToListAsync();
        return items.Select(e => new HrEmployeeListDto
        {
            Id = e.Id, EmployeeNumber = e.EmployeeNumber, FullName = e.FullName,
            DepartmentName = e.Department?.DepartmentName, JobTitleName = e.JobTitle?.TitleName,
            BranchName = e.Branch?.Name, Phone = e.Phone, Status = e.Status, HireDate = e.HireDate,
            EmployeeType = e.EmployeeType, Category = e.Category
        }).ToList();
    }

    public async Task<string> GenerateEmployeeNumberAsync()
    {
        var lastEmployee = await _repository.Query()
            .OrderByDescending(e => e.Id)
            .FirstOrDefaultAsync();
        var nextNumber = lastEmployee != null ? lastEmployee.Id + 1 : 1;
        return $"EMP-{nextNumber:D4}";
    }
}
