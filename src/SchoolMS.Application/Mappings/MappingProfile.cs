using AutoMapper;
using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Entities;

namespace SchoolMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch.Name))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null))
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.ClassRoom.Grade.GradeName))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.ClassRoom.Division.DivisionName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName));
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();

        // Teacher
        CreateMap<Teacher, TeacherDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch.Name))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<CreateTeacherDto, Teacher>();
        CreateMap<UpdateTeacherDto, Teacher>();

        // Subject
        CreateMap<Subject, SubjectDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<SubjectDto, Subject>();

        // Division
        CreateMap<Division, DivisionDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<DivisionDto, Division>();

        // Grade
        CreateMap<Grade, GradeDto>()
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.Division.DivisionName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<GradeDto, Grade>();

        // ClassRoom
        CreateMap<ClassRoom, ClassRoomDto>()
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.Grade.GradeName))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.Division.DivisionName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch.Name));
        CreateMap<ClassRoomDto, ClassRoom>();

        // AcademicYear
        CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();

        // Branch
        CreateMap<Branch, BranchDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<BranchDto, Branch>();

        // ExamType
        CreateMap<ExamType, ExamTypeDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<ExamTypeDto, ExamType>();

        // ExpenseType
        CreateMap<ExpenseType, ExpenseTypeDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<ExpenseTypeDto, ExpenseType>();

        // Attendance
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));

        // Expense
        CreateMap<Expense, ExpenseDto>()
            .ForMember(d => d.ExpenseTypeName, opt => opt.MapFrom(s => s.ExpenseType.TypeName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch.Name))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<ExpenseDto, Expense>();

        // FeeInstallment
        CreateMap<FeeInstallment, FeeInstallmentDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student.FullName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null))
            .ForMember(d => d.Payments, opt => opt.MapFrom(s => s.InstallmentPayments));
        CreateMap<InstallmentPayment, InstallmentPaymentDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.FeeInstallment.Student.FullName))
            .ForMember(d => d.StudentId, opt => opt.MapFrom(s => s.FeeInstallment.StudentId))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.FeeInstallment.AcademicYear.YearName))
            .ForMember(d => d.TotalInstallmentAmount, opt => opt.MapFrom(s => s.FeeInstallment.TotalAmount))
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.FeeInstallment.Student.ClassRoom != null ? s.FeeInstallment.Student.ClassRoom.Grade.GradeName : null))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.FeeInstallment.Student.ClassRoom != null ? s.FeeInstallment.Student.ClassRoom.Division.DivisionName : null))
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.FeeInstallment.Student.Phone))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));

        // ExamSchedule
        CreateMap<ExamSchedule, ExamScheduleDto>()
            .ForMember(d => d.ExamTypeName, opt => opt.MapFrom(s => s.ExamType.TypeName))
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.ClassRoom.Grade.GradeName))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.ClassRoom.Division.DivisionName))
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject.SubjectName))
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s => s.Teacher.FullName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.ClassRoom.Branch != null ? s.ClassRoom.Branch.Name : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<ExamScheduleDto, ExamSchedule>();

        // WeeklySchedule
        CreateMap<WeeklySchedule, WeeklyScheduleDto>()
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.ClassRoom.Grade.GradeName))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.ClassRoom.Division.DivisionName))
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject.SubjectName))
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s => s.Teacher.FullName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.ClassRoom.Branch != null ? s.ClassRoom.Branch.Name : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<WeeklyScheduleDto, WeeklySchedule>();

        // StudentGrade
        CreateMap<StudentGrade, StudentGradeDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student.FullName))
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject.SubjectName))
            .ForMember(d => d.ExamTypeName, opt => opt.MapFrom(s => s.ExamType.TypeName))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<StudentGradeDto, StudentGrade>();

        // SalarySetup
        CreateMap<SalarySetup, SalarySetupDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<SalarySetupDto, SalarySetup>();

        // LeaveRequest
        CreateMap<LeaveRequest, LeaveRequestDto>();
        CreateMap<LeaveRequestDto, LeaveRequest>();

        // Notification
        CreateMap<Notification, NotificationDto>().ReverseMap();

        // TeacherAssignment
        CreateMap<TeacherAssignment, TeacherAssignmentDto>()
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s => s.Teacher.FullName))
            .ForMember(d => d.GradeName, opt => opt.MapFrom(s => s.ClassRoom.Grade.GradeName))
            .ForMember(d => d.DivisionName, opt => opt.MapFrom(s => s.ClassRoom.Division.DivisionName))
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject.SubjectName))
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear.YearName))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch.Name))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<TeacherAssignmentDto, TeacherAssignment>();

        // ============ HR MODULE MAPPINGS ============
        CreateMap<HrDepartment, HrDepartmentDto>()
            .ForMember(d => d.ParentDepartmentName, opt => opt.MapFrom(s => s.ParentDepartment != null ? s.ParentDepartment.DepartmentName : null))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null))
            .ForMember(d => d.EmployeeCount, opt => opt.MapFrom(s => s.Employees != null ? s.Employees.Count : 0));
        CreateMap<HrDepartmentDto, HrDepartment>();

        CreateMap<HrJobTitle, HrJobTitleDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null));
        CreateMap<HrJobTitleDto, HrJobTitle>();

        CreateMap<HrJobGrade, HrJobGradeDto>().ReverseMap();
        CreateMap<HrJobGradeStep, HrJobGradeStepDto>().ReverseMap();

        CreateMap<HrEmployee, HrEmployeeDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.JobTitleName, opt => opt.MapFrom(s => s.JobTitle != null ? s.JobTitle.TitleName : null))
            .ForMember(d => d.JobGradeName, opt => opt.MapFrom(s => s.JobGrade != null ? s.JobGrade.GradeName : null))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null))
            .ForMember(d => d.WorkShiftName, opt => opt.MapFrom(s => s.WorkShift != null ? s.WorkShift.ShiftName : null))
            .ForMember(d => d.DirectManagerName, opt => opt.MapFrom(s => s.DirectManager != null ? s.DirectManager.FullName : null));
        CreateMap<HrEmployeeDto, HrEmployee>();

        CreateMap<HrEmployeeContract, HrEmployeeContractDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrEmployeeContractDto, HrEmployeeContract>();

        CreateMap<HrEmployeeDocument, HrEmployeeDocumentDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrEmployeeDocumentDto, HrEmployeeDocument>();

        CreateMap<HrWorkShift, HrWorkShiftDto>().ReverseMap();

        CreateMap<HrFingerprintDevice, HrFingerprintDeviceDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null));
        CreateMap<HrFingerprintDeviceDto, HrFingerprintDevice>();

        CreateMap<HrFingerprintRecord, HrFingerprintRecordDto>().ReverseMap();
        CreateMap<HrDailyAttendance, HrDailyAttendanceDto>().ReverseMap();

        CreateMap<HrOvertimeRequest, HrOvertimeRequestDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrOvertimeRequestDto, HrOvertimeRequest>();

        CreateMap<HrSalaryDetail, HrSalaryDetailDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrSalaryDetailDto, HrSalaryDetail>();

        CreateMap<HrAllowanceType, HrAllowanceTypeDto>().ReverseMap();
        CreateMap<HrSalaryAllowance, HrSalaryAllowanceDto>().ReverseMap();
        CreateMap<HrDeductionType, HrDeductionTypeDto>().ReverseMap();
        CreateMap<HrSalaryDeduction, HrSalaryDeductionDto>().ReverseMap();

        CreateMap<HrMonthlyPayroll, HrMonthlyPayrollDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null));
        CreateMap<HrPayrollItem, HrPayrollItemDto>().ReverseMap();

        CreateMap<HrSalaryAdvance, HrSalaryAdvanceDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrSalaryAdvanceDto, HrSalaryAdvance>();

        CreateMap<HrEmployeeLoan, HrEmployeeLoanDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrEmployeeLoanDto, HrEmployeeLoan>();

        CreateMap<HrBonus, HrBonusDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrBonusDto, HrBonus>();

        CreateMap<HrPenalty, HrPenaltyDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrPenaltyDto, HrPenalty>();

        CreateMap<HrPromotion, HrPromotionDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrPromotionDto, HrPromotion>();

        CreateMap<HrCareerHistory, HrCareerHistoryDto>().ReverseMap();

        CreateMap<HrLeaveType, HrLeaveTypeDto>().ReverseMap();
        CreateMap<HrLeaveRequest, HrLeaveRequestDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
            .ForMember(d => d.LeaveTypeName, opt => opt.MapFrom(s => s.LeaveType != null ? s.LeaveType.LeaveTypeName : null));
        CreateMap<HrLeaveRequestDto, HrLeaveRequest>();
        CreateMap<HrLeaveBalance, HrLeaveBalanceDto>().ReverseMap();

        CreateMap<HrHoliday, HrHolidayDto>()
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Branch != null ? s.Branch.Name : null));
        CreateMap<HrHolidayDto, HrHoliday>();

        CreateMap<HrPerformanceCycle, HrPerformanceCycleDto>().ReverseMap();
        CreateMap<HrPerformanceCriteria, HrPerformanceCriteriaDto>().ReverseMap();
        CreateMap<HrPerformanceReview, HrPerformanceReviewDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
            .ForMember(d => d.CycleName, opt => opt.MapFrom(s => s.PerformanceCycle != null ? s.PerformanceCycle.CycleName : null));
        CreateMap<HrPerformanceReviewDto, HrPerformanceReview>();
        CreateMap<HrPerformanceScore, HrPerformanceScoreDto>()
            .ForMember(d => d.CriteriaName, opt => opt.MapFrom(s => s.Criteria != null ? s.Criteria.CriteriaName : null));
        CreateMap<HrPerformanceScoreDto, HrPerformanceScore>();

        CreateMap<HrKpi, HrKpiDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrKpiDto, HrKpi>();

        CreateMap<HrTrainingProgram, HrTrainingProgramDto>().ReverseMap();
        CreateMap<HrTrainingRecord, HrTrainingRecordDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
            .ForMember(d => d.ProgramName, opt => opt.MapFrom(s => s.TrainingProgram != null ? s.TrainingProgram.ProgramName : null));
        CreateMap<HrTrainingRecordDto, HrTrainingRecord>();
        CreateMap<HrTrainingRequest, HrTrainingRequestDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrTrainingRequestDto, HrTrainingRequest>();
        CreateMap<HrProfessionalCertificate, HrProfessionalCertificateDto>().ReverseMap();

        CreateMap<HrViolationType, HrViolationTypeDto>().ReverseMap();
        CreateMap<HrDisciplinaryAction, HrDisciplinaryActionDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
            .ForMember(d => d.ViolationName, opt => opt.MapFrom(s => s.ViolationType != null ? s.ViolationType.ViolationName : null));
        CreateMap<HrDisciplinaryActionDto, HrDisciplinaryAction>();

        CreateMap<HrEndOfService, HrEndOfServiceDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrEndOfServiceDto, HrEndOfService>();

        CreateMap<HrEmployeeRequest, HrEmployeeRequestDto>()
            .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null));
        CreateMap<HrEmployeeRequestDto, HrEmployeeRequest>();

        // ============ TEACHER EARNING MAPPINGS ============
        CreateMap<TeacherEarning, TeacherEarningDto>()
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s => s.Teacher != null ? s.Teacher.FullName : null))
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course != null ? s.Course.Title : null))
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.StudentSubscription != null && s.StudentSubscription.Student != null ? s.StudentSubscription.Student.FullName : null))
            .ForMember(d => d.PlanName, opt => opt.MapFrom(s => s.StudentSubscription != null && s.StudentSubscription.OnlineSubscriptionPlan != null ? s.StudentSubscription.OnlineSubscriptionPlan.PlanName : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null));
        CreateMap<TeacherEarningDto, TeacherEarning>();

        // ============ ONLINE SUBSCRIPTION MAPPINGS ============
        CreateMap<OnlineSubscriptionPlan, OnlineSubscriptionPlanDto>()
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject != null ? s.Subject.SubjectName : null))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null))
            .ForMember(d => d.StudentSubscriptionCount, opt => opt.MapFrom(s => s.StudentSubscriptions != null ? s.StudentSubscriptions.Count : 0));
        CreateMap<OnlineSubscriptionPlanDto, OnlineSubscriptionPlan>();

        CreateMap<StudentSubscription, StudentSubscriptionDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student != null ? s.Student.FullName : null))
            .ForMember(d => d.PlanName, opt => opt.MapFrom(s => s.OnlineSubscriptionPlan != null ? s.OnlineSubscriptionPlan.PlanName : null))
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.OnlineSubscriptionPlan != null && s.OnlineSubscriptionPlan.Subject != null ? s.OnlineSubscriptionPlan.Subject.SubjectName : null))
            .ForMember(d => d.SubscriptionType, opt => opt.MapFrom(s => s.OnlineSubscriptionPlan != null ? s.OnlineSubscriptionPlan.SubscriptionType : default))
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null))
            .ForMember(d => d.BranchName, opt => opt.MapFrom(s => s.Student != null && s.Student.Branch != null ? s.Student.Branch.Name : null));
        CreateMap<StudentSubscriptionDto, StudentSubscription>();

        // ============ PROMO CODE MAPPINGS ============
        CreateMap<PromoCode, PromoCodeDto>()
            .ForMember(d => d.SchoolName, opt => opt.MapFrom(s => s.School != null ? s.School.Name : null))
            .ForMember(d => d.Usages, opt => opt.MapFrom(s => s.Usages));
        CreateMap<PromoCodeDto, PromoCode>();

        CreateMap<PromoCodeUsage, PromoCodeUsageDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student != null ? s.Student.FullName : null))
            .ForMember(d => d.PromoCodeText, opt => opt.MapFrom(s => s.PromoCode != null ? s.PromoCode.Code : null));
        CreateMap<PromoCodeUsageDto, PromoCodeUsage>();
    }
}
