using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface ILeaveService
{
    Task<List<LeaveRequestDto>> GetAllAsync();
    Task<List<LeaveRequestDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<LeaveRequestDto>> GetByPersonAsync(int personId, PersonType personType, int schoolId);
    Task<List<LeaveRequestDto>> GetByParentChildrenAsync(int parentId, int schoolId);
    Task<LeaveRequestDto?> GetByIdAsync(int id);
    Task<LeaveRequestDto> CreateAsync(LeaveRequestDto dto);
    Task<LeaveRequestDto> UpdateAsync(LeaveRequestDto dto);
    Task DeleteAsync(int id);
    Task ApproveAsync(int id);
    Task RejectAsync(int id);
}

