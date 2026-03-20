namespace SchoolMS.Domain.Entities;

public class OtpVerification
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int AttemptCount { get; set; }
    public bool IsBlocked { get; set; }
    public string? Purpose { get; set; }
}
