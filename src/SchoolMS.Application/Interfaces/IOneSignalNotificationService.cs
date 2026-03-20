namespace SchoolMS.Application.Interfaces;

public interface IOneSignalNotificationService
{
    /// <summary>
    /// Send notification to specific person types (Parent, Teacher, Student, Staff) filtered by school.
    /// </summary>
    Task SendToPersonTypesAsync(string title, string message, IEnumerable<string> personTypes, int schoolId);

    /// <summary>
    /// Send notification to specific person types filtered by school and classroom.
    /// </summary>
    Task SendToClassRoomAsync(string title, string message, IEnumerable<string> personTypes, int schoolId, int classRoomId);

    /// <summary>
    /// Send notification to a specific individual by person ID and type.
    /// </summary>
    Task SendToIndividualAsync(string title, string message, int personId, string personType, int schoolId);

    /// <summary>
    /// Send notification to all users of a school.
    /// </summary>
    Task SendToSchoolAsync(string title, string message, int schoolId);
}
