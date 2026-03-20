using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

public class SchoolRegistrationRequestDto
{
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? SchoolType { get; set; }
    public string? StudentCountRange { get; set; }
    public string? RequestedPlan { get; set; }
    public string? Notes { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? AssignedTo { get; set; }
    public string? InternalNotes { get; set; }
    public DateTime? ContactedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CreatedSchoolId { get; set; }
    public string? IpAddress { get; set; }
    public string? Source { get; set; }
}

public class CreateRegistrationRequestDto
{
    public string SchoolName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? SchoolType { get; set; }
    public string? StudentCountRange { get; set; }
    public string? RequestedPlan { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRequestStatusDto
{
    public int Id { get; set; }
    public RequestStatus Status { get; set; }
    public string? InternalNotes { get; set; }
}
