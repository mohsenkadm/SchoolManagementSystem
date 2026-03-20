using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrFingerprintDevice : BaseEntity
{
    public string DeviceName { get; set; } = string.Empty;
    public string? DeviceModel { get; set; }
    public string? SerialNumber { get; set; }
    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public string? Location { get; set; }
    public int BranchId { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public string? ConnectionType { get; set; }
    public bool IsActive { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
