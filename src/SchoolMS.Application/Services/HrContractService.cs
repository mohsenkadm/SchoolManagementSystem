using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrContractService : IHrContractService
{
    private readonly IRepository<HrEmployeeContract> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrContractService(IRepository<HrEmployeeContract> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrEmployeeContractDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(c => c.Employee).OrderByDescending(c => c.StartDate).ToListAsync();
        return MapContracts(items);
    }

    public async Task<List<HrEmployeeContractDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query().Where(c => c.SchoolId == schoolId).Include(c => c.Employee).OrderByDescending(c => c.StartDate).ToListAsync();
        return MapContracts(items);
    }

    private static List<HrEmployeeContractDto> MapContracts(List<HrEmployeeContract> items)
    {
        return items.Select(c => new HrEmployeeContractDto
        {
            Id = c.Id, EmployeeId = c.EmployeeId, EmployeeName = c.Employee?.FullName,
            ContractNumber = c.ContractNumber, ContractType = c.ContractType, StartDate = c.StartDate,
            EndDate = c.EndDate, AgreedSalary = c.AgreedSalary, Status = c.Status, Notes = c.Notes,
            SignedBy = c.SignedBy, SignedDate = c.SignedDate
        }).ToList();
    }

    public async Task<List<HrEmployeeContractDto>> GetByEmployeeAsync(int employeeId)
    {
        var items = await _repository.Query().Where(c => c.EmployeeId == employeeId).OrderByDescending(c => c.StartDate).ToListAsync();
        return _mapper.Map<List<HrEmployeeContractDto>>(items);
    }

    public async Task<HrEmployeeContractDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<HrEmployeeContractDto>(entity);
    }

    public async Task<HrEmployeeContractDto> CreateAsync(HrEmployeeContractDto dto)
    {
        var entity = _mapper.Map<HrEmployeeContract>(dto);
        entity.Id = 0;
        entity.Status = ContractStatus.Active;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEmployeeContractDto>(entity);
    }

    public async Task<HrEmployeeContractDto> UpdateAsync(HrEmployeeContractDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Contract with ID {dto.Id} not found.");
        entity.ContractType = dto.ContractType;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.AgreedSalary = dto.AgreedSalary;
        entity.Terms = dto.Terms;
        entity.Status = dto.Status;
        entity.Notes = dto.Notes;
        entity.SignedBy = dto.SignedBy;
        entity.SignedDate = dto.SignedDate;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrEmployeeContractDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contract with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
