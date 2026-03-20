using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrHoliday : BaseEntity
{
    public string HolidayName { get; set; } = string.Empty;
    public string? HolidayNameAr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public HolidayType Type { get; set; }
    public bool IsRecurring { get; set; }
    public int? BranchId { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }

    public virtual Branch? Branch { get; set; }
}
