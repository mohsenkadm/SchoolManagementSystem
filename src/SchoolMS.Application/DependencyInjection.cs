using Microsoft.Extensions.DependencyInjection;
using SchoolMS.Application.Interfaces;
using SchoolMS.Application.Mappings;
using SchoolMS.Application.Services;
using SchoolMS.Application.Settings;

namespace SchoolMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        // Core
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IDivisionService, DivisionService>();
        services.AddScoped<IGradeService, GradeService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IAcademicYearService, AcademicYearService>();
        services.AddScoped<IClassRoomService, ClassRoomService>();

        // Operational
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<IExamScheduleService, ExamScheduleService>();
        services.AddScoped<IWeeklyScheduleService, WeeklyScheduleService>();
        services.AddScoped<IStudentGradeService, StudentGradeService>();
        services.AddScoped<ISalaryService, SalaryService>();
        services.AddScoped<IInstallmentService, InstallmentService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IHomeworkService, HomeworkService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ICarouselService, CarouselService>();
        services.AddScoped<IExamTypeService, ExamTypeService>();
        services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IPromotionService, PromotionService>();

        // New Feature Modules
        services.AddScoped<IParentService, ParentService>();
        services.AddScoped<ISchoolEventService, SchoolEventService>();
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IStudentDocumentService, StudentDocumentService>();
        services.AddScoped<IStudentBehaviorService, StudentBehaviorService>();
        services.AddScoped<IHealthRecordService, HealthRecordService>();
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<ITransportService, TransportService>();
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        // Registration Requests
        services.AddScoped<IRegistrationRequestService, RegistrationRequestService>();

        // Courses & Online Platform
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseVideoService, CourseVideoService>();
        services.AddScoped<ILiveStreamService, LiveStreamService>();
        services.AddScoped<IOnlineSubscriptionPlanService, OnlineSubscriptionPlanService>();
        services.AddScoped<IStudentSubscriptionService, StudentSubscriptionService>();
        services.AddScoped<IPromoCodeService, PromoCodeService>();

        // OneSignal Push Notifications
        services.AddHttpClient<IOneSignalNotificationService, OneSignalNotificationService>();

        // Analytics
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        // Portal Auth
        services.AddScoped<IPortalAuthService, PortalAuthService>();

        // HR Module
        services.AddScoped<IHrDepartmentService, HrDepartmentService>();
        services.AddScoped<IHrJobTitleService, HrJobTitleService>();
        services.AddScoped<IHrJobGradeService, HrJobGradeService>();
        services.AddScoped<IHrEmployeeService, HrEmployeeService>();
        services.AddScoped<IHrContractService, HrContractService>();
        services.AddScoped<IHrWorkShiftService, HrWorkShiftService>();
        services.AddScoped<IHrFingerprintService, HrFingerprintService>();
        services.AddScoped<IHrAttendanceService, HrAttendanceService>();
        services.AddScoped<IHrOvertimeService, HrOvertimeService>();
        services.AddScoped<IHrSalaryService, HrSalaryService>();
        services.AddScoped<IHrPromotionService, HrPromotionService>();
        services.AddScoped<IHrLeaveService, HrLeaveService>();
        services.AddScoped<IHrPerformanceService, HrPerformanceService>();
        services.AddScoped<IHrTrainingService, HrTrainingService>();
        services.AddScoped<IHrDisciplinaryService, HrDisciplinaryService>();
        services.AddScoped<IHrEndOfServiceService, HrEndOfServiceService>();
        services.AddScoped<IHrDashboardService, HrDashboardService>();
        services.AddScoped<IHrSettingsService, HrSettingsService>();

        return services;
    }
}
