using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class StaffService : IStaffService
{
    private readonly IRepository<Staff> _repository;

    public StaffService(IRepository<Staff> repository) => _repository = repository;

    public async Task<List<StaffDto>> GetAllAsync()
    {
        var items = await _repository.Query().Where(s => !s.IsDeleted).ToListAsync();
        return items.Select(s => new StaffDto
        {
            Id = s.Id,
            FullName = s.FullName,
            Position = s.Position,
            Phone = s.Phone,
            BranchId = s.BranchId,
            BaseSalary = s.BaseSalary,
            SchoolId = s.SchoolId
        }).ToList();
    }
}
