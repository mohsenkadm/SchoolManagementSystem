using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

public class HomeworkDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public int AcademicYearId { get; set; }
    public int SchoolId { get; set; }
    public List<HomeworkAttachmentDto> Attachments { get; set; } = new();
}

public class HomeworkAttachmentDto
{
    public int Id { get; set; }
    public int HomeworkId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
}

public class QuizGroupDto
{
    public int Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int AcademicYearId { get; set; }
    public int SchoolId { get; set; }
    public int QuestionCount { get; set; }
}

public class QuizQuestionDto
{
    public int Id { get; set; }
    public int QuizGroupId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public int Points { get; set; }
}

public class SubmitQuizAnswerDto
{
    public int QuizQuestionId { get; set; }
    public string? Answer { get; set; }
}

public class QuizAnswerDto
{
    public int Id { get; set; }
    public int QuizQuestionId { get; set; }
    public string? QuestionText { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? Answer { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}

public class ChatRoomDto
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public ChatRoomType Type { get; set; }
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public int? ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public int? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public UserType UserType { get; set; }
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public List<int> PermissionIds { get; set; } = new();
    public List<int> BranchIds { get; set; } = new();
}

public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public int? BranchId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
    public List<int> BranchIds { get; set; } = new();
}

public class UpdateUserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public int? BranchId { get; set; }
    public string? NewPassword { get; set; }
    public List<int> PermissionIds { get; set; } = new();
    public List<int> BranchIds { get; set; } = new();
}

public class PermissionDto
{
    public int Id { get; set; }
    public string PageName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class StudentPromotionDto
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? FullName { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int FromClassRoomId { get; set; }
    public string? FromClassName { get; set; }
    public int ToClassRoomId { get; set; }
    public string? ToClassName { get; set; }
    public int FromAcademicYearId { get; set; }
    public int ToAcademicYearId { get; set; }
    public PromotionStatus Status { get; set; }
}

public class CarouselImageDto
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class UserDataTableDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class PromotionFilterDto
{
    public int FromAcademicYearId { get; set; }
    public int FromClassRoomId { get; set; }
}

public class PromotionExecuteDto
{
    public List<int> StudentIds { get; set; } = new();
    public int ToAcademicYearId { get; set; }
    public int ToClassRoomId { get; set; }
}
