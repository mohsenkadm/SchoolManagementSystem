using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IStaffService
{
    Task<List<StaffDto>> GetAllAsync();
}
