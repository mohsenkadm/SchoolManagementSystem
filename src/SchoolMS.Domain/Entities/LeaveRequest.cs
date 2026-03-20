using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public int PersonId { get; set; }
    public PersonType PersonType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; }
}
