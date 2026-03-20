namespace SchoolMS.Application.DTOs;

public class SendOtpRequestDto
{
    public string Phone { get; set; } = string.Empty;
    public string? Purpose { get; set; } = "StudentRegistration";
}

public class SendOtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class VerifyOtpRequestDto
{
    public string Phone { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class VerifyOtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? VerificationToken { get; set; }
}

public class RegisterStudentWithOtpDto
{
    public string VerificationToken { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? ParentPhone { get; set; }
    public string? ParentName { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public int ClassRoomId { get; set; }
    public int AcademicYearId { get; set; }
    public string? Notes { get; set; }
}

public class ResetPasswordDto
{
    public string Phone { get; set; } = string.Empty;
    public string VerificationToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
