using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceDto> CheckInAsync(CreateAttendanceDto dto);
    Task<BulkAttendanceItemDto?> ResolveBadgeAsync(string badgeCardNumber);
    Task<List<AttendanceDto>> SaveBulkAsync(BulkAttendanceSaveDto dto);
    Task<AttendanceReportDto> GetReportAsync(AttendanceFilterDto filter);
    Task<DataTableResponse<AttendanceDto>> GetDataTableAsync(DataTableRequest request);
    Task<byte[]> ExportToExcelAsync(AttendanceFilterDto? filter = null);
    Task<int> MarkAbsenteesAsync(int branchId, DateTime date);
    Task<List<AttendanceDto>> GetByPersonAsync(int personId, PersonType personType, int schoolId);
    Task<List<AttendanceDto>> GetByParentChildrenAsync(int parentId, int schoolId);
}

