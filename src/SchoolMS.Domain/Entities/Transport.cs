namespace SchoolMS.Domain.Entities;

public class TransportRoute : BaseEntity
{
    public string RouteName { get; set; } = string.Empty;
    public string? RouteNameAr { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? BusNumber { get; set; }
    public int Capacity { get; set; }
    public int CurrentPassengers { get; set; }
    public decimal MonthlyFee { get; set; }
    public int BranchId { get; set; }
    public bool IsActive { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<TransportStop> Stops { get; set; } = new List<TransportStop>();
    public virtual ICollection<StudentTransport> StudentTransports { get; set; } = new List<StudentTransport>();
}

public class TransportStop : BaseEntity
{
    public int TransportRouteId { get; set; }
    public string StopName { get; set; } = string.Empty;
    public string? StopNameAr { get; set; }
    public int StopOrder { get; set; }
    public TimeSpan PickupTime { get; set; }
    public TimeSpan DropoffTime { get; set; }

    public virtual TransportRoute TransportRoute { get; set; } = null!;
}

public class StudentTransport : BaseEntity
{
    public int StudentId { get; set; }
    public int TransportRouteId { get; set; }
    public int TransportStopId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual TransportRoute TransportRoute { get; set; } = null!;
    public virtual TransportStop TransportStop { get; set; } = null!;
}
