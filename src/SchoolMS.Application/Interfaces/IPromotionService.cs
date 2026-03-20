using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IPromotionService
{
    Task<List<StudentPromotionDto>> PreviewPromotionAsync(int fromClassRoomId, int toClassRoomId, int toAcademicYearId);
    Task ExecutePromotionAsync(List<StudentPromotionDto> promotions);
}

