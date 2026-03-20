using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IClassRoomService
{
    Task<List<ClassRoomDto>> GetAllAsync();
    Task<List<ClassRoomDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<ClassRoomDto?> GetByIdAsync(int id);
    Task<ClassRoomDto> CreateAsync(ClassRoomDto dto);
    Task<ClassRoomDto> UpdateAsync(ClassRoomDto dto);
    Task DeleteAsync(int id);
}

