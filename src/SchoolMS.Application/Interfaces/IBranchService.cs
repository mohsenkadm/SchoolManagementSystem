using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IBranchService
{
    Task<List<BranchDto>> GetAllAsync();
    Task<List<BranchDto>> GetBySchoolIdAsync(int schoolId);
    Task<BranchDto?> GetByIdAsync(int id);
    Task<BranchDto> CreateAsync(BranchDto dto);
    Task<BranchDto> UpdateAsync(BranchDto dto);
    Task DeleteAsync(int id);
}

