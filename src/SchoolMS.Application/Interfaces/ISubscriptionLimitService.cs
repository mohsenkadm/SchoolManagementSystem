namespace SchoolMS.Application.Interfaces;

public interface ISubscriptionLimitService
{
    Task<(bool allowed, string? error)> CanAddStudentAsync(int schoolId);
    Task<(bool allowed, string? error)> CanAddUserAsync(int schoolId);
    Task<SubscriptionStatus?> GetStatusAsync(int schoolId);
}

public class SubscriptionStatus
{
    public string PlanName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentStudents { get; set; }
    public int MaxUsers { get; set; }
    public int CurrentUsers { get; set; }
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
}
