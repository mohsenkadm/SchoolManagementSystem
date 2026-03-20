namespace SchoolMS.Domain.Enums;

public enum BehaviorType { Positive, Negative, Warning, Suspension }
public enum ComplaintCategory { Academic, Administrative, Facility, Behavior, Financial, Other }
public enum ComplaintPriority { Low, Medium, High, Critical }
public enum ComplaintStatus { Open, InProgress, Resolved, Closed, Reopened }
public enum VisitorStatus { CheckedIn, CheckedOut }
public enum BorrowStatus { Borrowed, Returned, Overdue, Lost }
public enum AssetStatus { Available, InUse, Maintenance, Disposed, Lost }
public enum AnnouncementPriority { Normal, Important, Urgent }
public enum AnnouncementTarget { All, Students, Teachers, Parents, Staff, Branch }
public enum EventType { Holiday, Meeting, Activity, Exam, Sports, Trip, Cultural, Other }
public enum StudentStatus { Active, Graduated, Withdrawn, Suspended, Transferred, Expelled }
