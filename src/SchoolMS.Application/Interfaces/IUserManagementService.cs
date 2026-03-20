using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IUserManagementService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto> UpdateUserAsync(UpdateUserDto dto);
    Task DeleteUserAsync(string id);
    Task<List<PermissionDto>> GetAllPermissionsAsync();
    Task UpdateUserPermissionsAsync(string userId, List<int> permissionIds);
    Task UpdateUserBranchesAsync(string userId, List<int> branchIds);
    Task<DataTableResponse<UserDataTableDto>> GetDataTableAsync(DataTableRequest request);
    Task<List<string>> GetAllRolesAsync();
}

