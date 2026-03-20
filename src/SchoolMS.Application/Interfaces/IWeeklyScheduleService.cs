using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IWeeklyScheduleService
{
    Task<List<WeeklyScheduleDto>> GetAllAsync();
    Task<List<WeeklyScheduleDto>> GetBySchoolIdAsync(int schoolId);
    Task<List<WeeklyScheduleDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId);
    Task<WeeklyScheduleDto?> GetByIdAsync(int id);
    Task<WeeklyScheduleDto> CreateAsync(WeeklyScheduleDto dto);
    Task CreateBulkAsync(List<WeeklyScheduleDto> dtos);
    Task<WeeklyScheduleDto> UpdateAsync(WeeklyScheduleDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportToExcelAsync();
}

