using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrFingerprintRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? BadgeCardNumber { get; set; }
    public DateTime RecordDate { get; set; }
    public TimeSpan RecordTime { get; set; }
    public DateTime RecordDateTime { get; set; }
    public FingerprintType Type { get; set; }
    public FingerprintSource Source { get; set; }
    public int? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public int BranchId { get; set; }
    public bool IsManualEntry { get; set; }
    public string? ManualEntryReason { get; set; }
    public string? EnteredBy { get; set; }
    public string? IpAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrFingerprintDevice? Device { get; set; }
}
