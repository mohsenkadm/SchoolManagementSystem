using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IPortalAuthService
{
    Task<PortalLoginResultDto> TeacherLoginAsync(PortalLoginDto dto);
    Task<PortalLoginResultDto> StudentLoginAsync(PortalLoginDto dto);
    Task<PortalLoginResultDto> ParentLoginAsync(PortalLoginDto dto);
    Task<PortalLoginResultDto> StaffLoginAsync(PortalLoginDto dto);
}
