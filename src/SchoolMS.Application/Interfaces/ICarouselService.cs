using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface ICarouselService
{
    Task<List<CarouselImageDto>> GetAllAsync();
    Task<List<CarouselImageDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<CarouselImageDto> CreateAsync(CarouselImageDto dto);
    Task<CarouselImageDto> UpdateAsync(CarouselImageDto dto);
    Task DeleteAsync(int id);
    Task ReorderAsync(List<int> ids);
}

