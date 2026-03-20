using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SchoolDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SchoolDbContext context,
        ITenantProvider tenantProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

        // School-level users should not see or assign the SuperAdmin role
        if (_tenantProvider.GetCurrentSchoolId().HasValue)
            roles = roles.Where(r => r != "SuperAdmin").ToList();

        return roles;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.Branch)
            .Include(u => u.UserPermissions)
            .Include(u => u.UserBranches)
            .ToListAsync();

        var result = new List<UserDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(MapToUserDto(u, roles));
        }
        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var u = await _context.Users
            .Include(u => u.Branch)
            .Include(u => u.UserPermissions)
            .Include(u => u.UserBranches)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (u == null) return null;

        var roles = await _userManager.GetRolesAsync(u);
        return MapToUserDto(u, roles);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var schoolId = _tenantProvider.GetCurrentSchoolId()
            ?? throw new InvalidOperationException("Cannot create user without a school context.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            UserType = dto.UserType,
            SchoolId = schoolId,
            BranchId = dto.BranchId,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        if (dto.PermissionIds.Count > 0)
            await UpdateUserPermissionsAsync(user.Id, dto.PermissionIds);

        if (dto.BranchIds.Count > 0)
            await UpdateUserBranchesAsync(user.Id, dto.BranchIds);

        return (await GetUserByIdAsync(user.Id))!;
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"User with ID {dto.Id} not found.");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.UserType = dto.UserType;
        user.BranchId = dto.BranchId;

        await _userManager.UpdateAsync(user);

        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        }

        await UpdateUserPermissionsAsync(user.Id, dto.PermissionIds);
        await UpdateUserBranchesAsync(user.Id, dto.BranchIds);

        return (await GetUserByIdAsync(user.Id))!;
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
    }

    public async Task<List<PermissionDto>> GetAllPermissionsAsync()
    {
        return await _context.Permissions
            .Where(p => !p.IsDeleted)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                PageName = p.PageName,
                Action = p.Action
            })
            .ToListAsync();
    }

    public async Task UpdateUserPermissionsAsync(string userId, List<int> permissionIds)
    {
        var existing = await _context.UserPermissions
            .Where(up => up.UserId == userId && !up.IsDeleted)
            .ToListAsync();

        foreach (var ep in existing)
        {
            ep.IsDeleted = true;
            ep.DeletedAt = DateTime.UtcNow;
        }

        foreach (var permId in permissionIds)
        {
            _context.UserPermissions.Add(new UserPermission
            {
                UserId = userId,
                PermissionId = permId,
                IsGranted = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserBranchesAsync(string userId, List<int> branchIds)
    {
        var existing = await _context.UserBranches
            .Where(ub => ub.UserId == userId && !ub.IsDeleted)
            .ToListAsync();

        foreach (var eb in existing)
        {
            eb.IsDeleted = true;
            eb.DeletedAt = DateTime.UtcNow;
        }

        foreach (var branchId in branchIds)
        {
            _context.UserBranches.Add(new UserBranch
            {
                UserId = userId,
                BranchId = branchId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<DataTableResponse<UserDataTableDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.Branch)
            .AsQueryable();

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(request.SearchValue))
        {
            var search = request.SearchValue.ToLower();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(search) ||
                (u.Email != null && u.Email.ToLower().Contains(search)));
        }

        var filteredRecords = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.FullName)
            .Skip(request.Start)
            .Take(request.Length)
            .ToListAsync();

        var data = new List<UserDataTableDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            data.Add(new UserDataTableDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                UserType = u.UserType.ToString(),
                Roles = string.Join(", ", roles),
                IsActive = !u.IsDeleted
            });
        }

        return new DataTableResponse<UserDataTableDto>
        {
            Draw = request.Draw,
            RecordsTotal = totalRecords,
            RecordsFiltered = filteredRecords,
            Data = data
        };
    }

    private static UserDto MapToUserDto(ApplicationUser u, IList<string> roles) => new()
    {
        Id = u.Id,
        UserName = u.UserName ?? string.Empty,
        Email = u.Email ?? string.Empty,
        FullName = u.FullName,
        ProfileImage = u.ProfileImage,
        UserType = u.UserType,
        SchoolId = u.SchoolId,
        BranchId = u.BranchId,
        BranchName = u.Branch?.Name,
        PermissionIds = u.UserPermissions.Where(p => p.IsGranted && !p.IsDeleted).Select(p => p.PermissionId).ToList(),
        BranchIds = u.UserBranches.Where(b => !b.IsDeleted).Select(b => b.BranchId).ToList()
    };
}
