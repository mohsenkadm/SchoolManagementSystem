using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IVisitorService
{
    Task<List<VisitorDto>> GetAllAsync();
    Task<VisitorDto?> GetByIdAsync(int id);
    Task<VisitorDto> CheckInAsync(VisitorDto dto);
    Task CheckOutAsync(int id);
}

