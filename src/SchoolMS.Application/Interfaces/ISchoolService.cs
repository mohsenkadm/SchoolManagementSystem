using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ISchoolService
{
    Task<List<SchoolDto>> GetBySchoolIdAsync(int schoolId);     
}

