using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHrModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludesHrModule",
                table: "SystemSubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "HrAbsenceDeductionPerDay",
                table: "Schools",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "HrAbsenceDeductionType",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HrAutoCalculateOvertime",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HrAutoDeductAbsence",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "HrEarlyLeaveDeductionPerMinute",
                table: "Schools",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HrEnableFingerprintIntegration",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HrEnableSelfService",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "HrLateDeductionPerMinute",
                table: "Schools",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "HrLateGracePeriodMinutes",
                table: "Schools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HrMaxOvertimeHoursPerMonth",
                table: "Schools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "HrOvertimeRateMultiplier",
                table: "Schools",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HrRequireApprovalForLeaves",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HrSalaryCalculationMethod",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HrWorkDayEnd",
                table: "Schools",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HrWorkDayStart",
                table: "Schools",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "HrWorkingDaysPerMonth",
                table: "Schools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHrModuleEnabled",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HrAllowanceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowanceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowanceNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowanceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrAllowanceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrAllowanceTypes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrDeductionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeductionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeductionNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeductionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrDeductionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrDeductionTypes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ManagerEmployeeId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrDepartments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDepartments_HrDepartments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "HrDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDepartments_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrFingerprintDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConnectionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrFingerprintDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrFingerprintDevices_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrFingerprintDevices_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrHolidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HolidayNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrHolidays_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrHolidays_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrJobGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradeNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeLevel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DefaultAllowancePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinYearsExperience = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrJobGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrJobGrades_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrLeaveTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeaveTypeNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultDaysPerYear = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequiresDocument = table.Column<bool>(type: "bit", nullable: false),
                    DeductsFromSalary = table.Column<bool>(type: "bit", nullable: false),
                    DeductionPerDay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AllowHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    AllowNegativeBalance = table.Column<bool>(type: "bit", nullable: false),
                    MaxConsecutiveDays = table.Column<int>(type: "int", nullable: true),
                    MinAdvanceNoticeDays = table.Column<int>(type: "int", nullable: true),
                    CarryForward = table.Column<bool>(type: "bit", nullable: false),
                    MaxCarryForwardDays = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicableFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLeaveTypes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrMonthlyPayrolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PayrollPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    TotalEmployees = table.Column<int>(type: "int", nullable: false),
                    TotalBaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAllowances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalBonuses = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPenalties = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalOvertimeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAbsenceDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalLateDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAdvanceDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalLoanDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNetSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalGrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreparedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrMonthlyPayrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrMonthlyPayrolls_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrMonthlyPayrolls_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPerformanceCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriteriaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CriteriaNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPerformanceCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPerformanceCriterias_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPerformanceCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CycleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CycleNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPerformanceCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPerformanceCycles_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrTrainingPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trainer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CostPerParticipant = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateTemplatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTrainingPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrTrainingPrograms_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrViolationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViolationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViolationNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    DefaultAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultPenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DefaultSuspensionDays = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrViolationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrViolationTypes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrWorkShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalWorkHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GracePeriodMinutes = table.Column<int>(type: "int", nullable: false),
                    EarlyLeaveGraceMinutes = table.Column<int>(type: "int", nullable: false),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false),
                    FlexStartFrom = table.Column<TimeSpan>(type: "time", nullable: true),
                    FlexStartTo = table.Column<TimeSpan>(type: "time", nullable: true),
                    WorkingDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNightShift = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrWorkShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrWorkShifts_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtpVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolRegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentCountRange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedSchoolId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolRegistrationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolRegistrationRequests_Schools_CreatedSchoolId",
                        column: x => x.CreatedSchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HrJobTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrJobTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrJobTitles_HrDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "HrDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrJobTitles_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrJobGradeSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AnnualIncrement = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    YearsInStep = table.Column<int>(type: "int", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrJobGradeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrJobGradeSteps_HrJobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "HrJobGrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrJobGradeSteps_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThirdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfDependents = table.Column<int>(type: "int", nullable: true),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactRelation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: false),
                    JobGradeId = table.Column<int>(type: "int", nullable: true),
                    JobGradeStepId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    WorkShiftId = table.Column<int>(type: "int", nullable: true),
                    EmployeeType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProbationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DirectManagerId = table.Column<int>(type: "int", nullable: true),
                    ReportsToEmployeeId = table.Column<int>(type: "int", nullable: true),
                    BadgeCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FingerprintId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BarcodeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HighestQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GraduationYear = table.Column<int>(type: "int", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployees_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "HrDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrEmployees_DirectManagerId",
                        column: x => x.DirectManagerId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrJobGradeSteps_JobGradeStepId",
                        column: x => x.JobGradeStepId,
                        principalTable: "HrJobGradeSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrJobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "HrJobGrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrJobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "HrJobTitles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_HrWorkShifts_WorkShiftId",
                        column: x => x.WorkShiftId,
                        principalTable: "HrWorkShifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployees_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrBonuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BonusType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncludeInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrBonuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrBonuses_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrBonuses_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrCareerHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrCareerHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrCareerHistories_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrCareerHistories_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrDailyAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkShiftId = table.Column<int>(type: "int", nullable: true),
                    FirstCheckIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastCheckOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    ScheduledStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    ScheduledEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    TotalWorkHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RequiredWorkHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OvertimeHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ShortageHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LateMinutes = table.Column<int>(type: "int", nullable: true),
                    EarlyLeaveMinutes = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsLate = table.Column<bool>(type: "bit", nullable: false),
                    IsEarlyLeave = table.Column<bool>(type: "bit", nullable: false),
                    IsAbsent = table.Column<bool>(type: "bit", nullable: false),
                    IsOvertime = table.Column<bool>(type: "bit", nullable: false),
                    IsOnLeave = table.Column<bool>(type: "bit", nullable: false),
                    IsHoliday = table.Column<bool>(type: "bit", nullable: false),
                    IsWeekend = table.Column<bool>(type: "bit", nullable: false),
                    IsExcused = table.Column<bool>(type: "bit", nullable: false),
                    LateDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EarlyLeaveDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AbsenceDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OvertimeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrDailyAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrDailyAttendances_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDailyAttendances_HrWorkShifts_WorkShiftId",
                        column: x => x.WorkShiftId,
                        principalTable: "HrWorkShifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDailyAttendances_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrDisciplinaryActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ViolationTypeId = table.Column<int>(type: "int", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncidentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    WarningLevel = table.Column<int>(type: "int", nullable: true),
                    SuspensionDays = table.Column<int>(type: "int", nullable: true),
                    SuspensionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuspensionEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaidSuspension = table.Column<bool>(type: "bit", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EmployeeResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Witnesses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evidence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyEmployee = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrDisciplinaryActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrDisciplinaryActions_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDisciplinaryActions_HrViolationTypes_ViolationTypeId",
                        column: x => x.ViolationTypeId,
                        principalTable: "HrViolationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrDisciplinaryActions_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEmployeeContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreedSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RenewedFromContractId = table.Column<int>(type: "int", nullable: true),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployeeContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployeeContracts_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeContracts_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEmployeeDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployeeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployeeDocuments_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeDocuments_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEmployeeLoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRepayment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RepaymentMonths = table.Column<int>(type: "int", nullable: false),
                    MonthlyInstallment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidSoFar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstInstallmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GuarantorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuarantorPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployeeLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployeeLoans_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeLoans_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEmployeeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEmployeeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEmployeeRequests_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEmployeeRequests_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrEndOfServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastWorkingDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoticePeriodDays = table.Column<int>(type: "int", nullable: false),
                    IsNoticePeriodServed = table.Column<bool>(type: "bit", nullable: false),
                    TotalServiceYears = table.Column<int>(type: "int", nullable: false),
                    TotalServiceMonths = table.Column<int>(type: "int", nullable: false),
                    TotalServiceDays = table.Column<int>(type: "int", nullable: false),
                    LastBaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EndOfServiceBenefit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnusedLeaveCompensation = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingBonuses = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingAllowances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEntitlements = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingAdvances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingLoans = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingPenalties = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalSettlementAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetsClearance = table.Column<bool>(type: "bit", nullable: false),
                    FinanceClearance = table.Column<bool>(type: "bit", nullable: false),
                    ItClearance = table.Column<bool>(type: "bit", nullable: false),
                    HrClearance = table.Column<bool>(type: "bit", nullable: false),
                    AllClearancesCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExitInterviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExitInterviewRating = table.Column<int>(type: "int", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrEndOfServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrEndOfServices_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrEndOfServices_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrFingerprintRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BadgeCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RecordDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    IsManualEntry = table.Column<bool>(type: "bit", nullable: false),
                    ManualEntryReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnteredBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrFingerprintRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrFingerprintRecords_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrFingerprintRecords_HrFingerprintDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "HrFingerprintDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrFingerprintRecords_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrKpis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    KpiName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeasurementUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AchievementPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PerformanceCycleId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrKpis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrKpis_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrKpis_HrPerformanceCycles_PerformanceCycleId",
                        column: x => x.PerformanceCycleId,
                        principalTable: "HrPerformanceCycles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrKpis_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrLeaveBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalEntitlement = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CarriedForward = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAvailable = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Used = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Pending = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLeaveBalances_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrLeaveBalances_HrLeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "HrLeaveTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrLeaveBalances_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrLeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    HalfDayPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubstituteEmployeeId = table.Column<int>(type: "int", nullable: true),
                    ApprovedByManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByHR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLeaveRequests_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrLeaveRequests_HrLeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "HrLeaveTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrLeaveRequests_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrOvertimeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RateMultiplier = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalculatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFromAttendance = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrOvertimeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrOvertimeRequests_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrOvertimeRequests_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPayrollItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlyPayrollId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HousingAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FoodAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhoneAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PositionAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FamilyAllowance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAllowances = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AllowancesBreakdown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialSecurityDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InsuranceDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFixedDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionsBreakdown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    PresentDays = table.Column<int>(type: "int", nullable: false),
                    AbsentDays = table.Column<int>(type: "int", nullable: false),
                    LateDays = table.Column<int>(type: "int", nullable: false),
                    EarlyLeaveDays = table.Column<int>(type: "int", nullable: false),
                    LeaveDays = table.Column<int>(type: "int", nullable: false),
                    HolidayDays = table.Column<int>(type: "int", nullable: false),
                    TotalLateMinutes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEarlyLeaveMinutes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalOvertimeHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AbsenceDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LateDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EarlyLeaveDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OvertimeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BonusAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BonusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PenaltyReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvanceDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayrollItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayrollItems_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPayrollItems_HrMonthlyPayrolls_MonthlyPayrollId",
                        column: x => x.MonthlyPayrollId,
                        principalTable: "HrMonthlyPayrolls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPayrollItems_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PenaltyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViolationDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncludeInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisciplinaryActionId = table.Column<int>(type: "int", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPenalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPenalties_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPenalties_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPerformanceReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PerformanceCycleId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxPossibleScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PerformanceRating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSelfAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedSalaryIncrease = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPerformanceReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPerformanceReviews_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPerformanceReviews_HrPerformanceCycles_PerformanceCycleId",
                        column: x => x.PerformanceCycleId,
                        principalTable: "HrPerformanceCycles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPerformanceReviews_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrProfessionalCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrProfessionalCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrProfessionalCertificates_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrProfessionalCertificates_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FromJobTitleId = table.Column<int>(type: "int", nullable: true),
                    FromJobGradeId = table.Column<int>(type: "int", nullable: true),
                    FromJobGradeStepId = table.Column<int>(type: "int", nullable: true),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: true),
                    FromBranchId = table.Column<int>(type: "int", nullable: true),
                    FromSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ToJobTitleId = table.Column<int>(type: "int", nullable: true),
                    ToJobGradeId = table.Column<int>(type: "int", nullable: true),
                    ToJobGradeStepId = table.Column<int>(type: "int", nullable: true),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ToBranchId = table.Column<int>(type: "int", nullable: true),
                    ToSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryIncrease = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryIncreasePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPromotions_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPromotions_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryAdvances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeductionMonths = table.Column<int>(type: "int", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductedSoFar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FirstDeductionMonth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryAdvances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryAdvances_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryAdvances_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalaryType = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryDetails_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryDetails_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrTrainingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TrainingProgramId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AttendancePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CertificateIssued = table.Column<bool>(type: "bit", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTrainingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrTrainingRecords_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrTrainingRecords_HrTrainingPrograms_TrainingProgramId",
                        column: x => x.TrainingProgramId,
                        principalTable: "HrTrainingPrograms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrTrainingRecords_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrTrainingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestedTraining = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PreferredStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTrainingProgramId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTrainingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrTrainingRequests_HrEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HrEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrTrainingRequests_HrTrainingPrograms_AssignedTrainingProgramId",
                        column: x => x.AssignedTrainingProgramId,
                        principalTable: "HrTrainingPrograms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrTrainingRequests_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrLoanRepaymentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeLoanId = table.Column<int>(type: "int", nullable: false),
                    PayrollItemId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingAfter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrLoanRepaymentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrLoanRepaymentLogs_HrEmployeeLoans_EmployeeLoanId",
                        column: x => x.EmployeeLoanId,
                        principalTable: "HrEmployeeLoans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrLoanRepaymentLogs_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrClearanceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndOfServiceId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrClearanceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrClearanceItems_HrEndOfServices_EndOfServiceId",
                        column: x => x.EndOfServiceId,
                        principalTable: "HrEndOfServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrClearanceItems_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrPerformanceScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerformanceReviewId = table.Column<int>(type: "int", nullable: false),
                    PerformanceCriteriaId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WeightedScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPerformanceScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPerformanceScores_HrPerformanceCriterias_PerformanceCriteriaId",
                        column: x => x.PerformanceCriteriaId,
                        principalTable: "HrPerformanceCriterias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPerformanceScores_HrPerformanceReviews_PerformanceReviewId",
                        column: x => x.PerformanceReviewId,
                        principalTable: "HrPerformanceReviews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrPerformanceScores_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrAdvanceDeductionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryAdvanceId = table.Column<int>(type: "int", nullable: false),
                    PayrollItemId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    DeductedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingAfter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrAdvanceDeductionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrAdvanceDeductionLogs_HrSalaryAdvances_SalaryAdvanceId",
                        column: x => x.SalaryAdvanceId,
                        principalTable: "HrSalaryAdvances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrAdvanceDeductionLogs_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryAllowances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryDetailId = table.Column<int>(type: "int", nullable: false),
                    AllowanceTypeId = table.Column<int>(type: "int", nullable: false),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalculatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryAllowances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryAllowances_HrAllowanceTypes_AllowanceTypeId",
                        column: x => x.AllowanceTypeId,
                        principalTable: "HrAllowanceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryAllowances_HrSalaryDetails_SalaryDetailId",
                        column: x => x.SalaryDetailId,
                        principalTable: "HrSalaryDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryAllowances_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryDetailId = table.Column<int>(type: "int", nullable: false),
                    DeductionTypeId = table.Column<int>(type: "int", nullable: false),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalculatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryDeductions_HrDeductionTypes_DeductionTypeId",
                        column: x => x.DeductionTypeId,
                        principalTable: "HrDeductionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryDeductions_HrSalaryDetails_SalaryDetailId",
                        column: x => x.SalaryDetailId,
                        principalTable: "HrSalaryDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HrSalaryDeductions_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrAdvanceDeductionLogs_SalaryAdvanceId",
                table: "HrAdvanceDeductionLogs",
                column: "SalaryAdvanceId");

            migrationBuilder.CreateIndex(
                name: "IX_HrAdvanceDeductionLogs_SchoolId",
                table: "HrAdvanceDeductionLogs",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrAllowanceTypes_SchoolId",
                table: "HrAllowanceTypes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrBonuses_EmployeeId",
                table: "HrBonuses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrBonuses_SchoolId",
                table: "HrBonuses",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrCareerHistories_EmployeeId",
                table: "HrCareerHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrCareerHistories_SchoolId",
                table: "HrCareerHistories",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrClearanceItems_EndOfServiceId",
                table: "HrClearanceItems",
                column: "EndOfServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HrClearanceItems_SchoolId",
                table: "HrClearanceItems",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDailyAttendances_EmployeeId_AttendanceDate",
                table: "HrDailyAttendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrDailyAttendances_SchoolId",
                table: "HrDailyAttendances",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDailyAttendances_WorkShiftId",
                table: "HrDailyAttendances",
                column: "WorkShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDeductionTypes_SchoolId",
                table: "HrDeductionTypes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDepartments_BranchId",
                table: "HrDepartments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDepartments_ParentDepartmentId",
                table: "HrDepartments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDepartments_SchoolId",
                table: "HrDepartments",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDisciplinaryActions_EmployeeId",
                table: "HrDisciplinaryActions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDisciplinaryActions_SchoolId",
                table: "HrDisciplinaryActions",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrDisciplinaryActions_ViolationTypeId",
                table: "HrDisciplinaryActions",
                column: "ViolationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeContracts_EmployeeId",
                table: "HrEmployeeContracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeContracts_SchoolId",
                table: "HrEmployeeContracts",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeDocuments_EmployeeId",
                table: "HrEmployeeDocuments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeDocuments_SchoolId",
                table: "HrEmployeeDocuments",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeLoans_EmployeeId",
                table: "HrEmployeeLoans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeLoans_SchoolId",
                table: "HrEmployeeLoans",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeRequests_EmployeeId",
                table: "HrEmployeeRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployeeRequests_SchoolId",
                table: "HrEmployeeRequests",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_BranchId",
                table: "HrEmployees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_DepartmentId",
                table: "HrEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_DirectManagerId",
                table: "HrEmployees",
                column: "DirectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_EmployeeNumber",
                table: "HrEmployees",
                column: "EmployeeNumber");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_JobGradeId",
                table: "HrEmployees",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_JobGradeStepId",
                table: "HrEmployees",
                column: "JobGradeStepId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_JobTitleId",
                table: "HrEmployees",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_NationalId",
                table: "HrEmployees",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_SchoolId",
                table: "HrEmployees",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEmployees_WorkShiftId",
                table: "HrEmployees",
                column: "WorkShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEndOfServices_EmployeeId",
                table: "HrEndOfServices",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrEndOfServices_SchoolId",
                table: "HrEndOfServices",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrFingerprintDevices_BranchId",
                table: "HrFingerprintDevices",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HrFingerprintDevices_SchoolId",
                table: "HrFingerprintDevices",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrFingerprintRecords_DeviceId",
                table: "HrFingerprintRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_HrFingerprintRecords_EmployeeId_RecordDate",
                table: "HrFingerprintRecords",
                columns: new[] { "EmployeeId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HrFingerprintRecords_SchoolId",
                table: "HrFingerprintRecords",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrHolidays_BranchId",
                table: "HrHolidays",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HrHolidays_SchoolId",
                table: "HrHolidays",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobGrades_SchoolId",
                table: "HrJobGrades",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobGradeSteps_JobGradeId",
                table: "HrJobGradeSteps",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobGradeSteps_SchoolId",
                table: "HrJobGradeSteps",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobTitles_DepartmentId",
                table: "HrJobTitles",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobTitles_SchoolId",
                table: "HrJobTitles",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrKpis_EmployeeId",
                table: "HrKpis",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrKpis_PerformanceCycleId",
                table: "HrKpis",
                column: "PerformanceCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrKpis_SchoolId",
                table: "HrKpis",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveBalances_EmployeeId_LeaveTypeId_Year",
                table: "HrLeaveBalances",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveBalances_LeaveTypeId",
                table: "HrLeaveBalances",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveBalances_SchoolId",
                table: "HrLeaveBalances",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_EmployeeId",
                table: "HrLeaveRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_LeaveTypeId",
                table: "HrLeaveRequests",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveRequests_SchoolId",
                table: "HrLeaveRequests",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLeaveTypes_SchoolId",
                table: "HrLeaveTypes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLoanRepaymentLogs_EmployeeLoanId",
                table: "HrLoanRepaymentLogs",
                column: "EmployeeLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_HrLoanRepaymentLogs_SchoolId",
                table: "HrLoanRepaymentLogs",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrMonthlyPayrolls_BranchId",
                table: "HrMonthlyPayrolls",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HrMonthlyPayrolls_SchoolId_BranchId_Month_Year",
                table: "HrMonthlyPayrolls",
                columns: new[] { "SchoolId", "BranchId", "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrOvertimeRequests_EmployeeId",
                table: "HrOvertimeRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrOvertimeRequests_SchoolId",
                table: "HrOvertimeRequests",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollItems_EmployeeId",
                table: "HrPayrollItems",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollItems_MonthlyPayrollId",
                table: "HrPayrollItems",
                column: "MonthlyPayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollItems_SchoolId",
                table: "HrPayrollItems",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPenalties_EmployeeId",
                table: "HrPenalties",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPenalties_SchoolId",
                table: "HrPenalties",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceCriterias_SchoolId",
                table: "HrPerformanceCriterias",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceCycles_SchoolId",
                table: "HrPerformanceCycles",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceReviews_EmployeeId",
                table: "HrPerformanceReviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceReviews_PerformanceCycleId",
                table: "HrPerformanceReviews",
                column: "PerformanceCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceReviews_SchoolId",
                table: "HrPerformanceReviews",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceScores_PerformanceCriteriaId",
                table: "HrPerformanceScores",
                column: "PerformanceCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceScores_PerformanceReviewId",
                table: "HrPerformanceScores",
                column: "PerformanceReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPerformanceScores_SchoolId",
                table: "HrPerformanceScores",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrProfessionalCertificates_EmployeeId",
                table: "HrProfessionalCertificates",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrProfessionalCertificates_SchoolId",
                table: "HrProfessionalCertificates",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPromotions_EmployeeId",
                table: "HrPromotions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPromotions_SchoolId",
                table: "HrPromotions",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryAdvances_EmployeeId",
                table: "HrSalaryAdvances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryAdvances_SchoolId",
                table: "HrSalaryAdvances",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryAllowances_AllowanceTypeId",
                table: "HrSalaryAllowances",
                column: "AllowanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryAllowances_SalaryDetailId",
                table: "HrSalaryAllowances",
                column: "SalaryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryAllowances_SchoolId",
                table: "HrSalaryAllowances",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryDeductions_DeductionTypeId",
                table: "HrSalaryDeductions",
                column: "DeductionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryDeductions_SalaryDetailId",
                table: "HrSalaryDeductions",
                column: "SalaryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryDeductions_SchoolId",
                table: "HrSalaryDeductions",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryDetails_EmployeeId",
                table: "HrSalaryDetails",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryDetails_SchoolId",
                table: "HrSalaryDetails",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingPrograms_SchoolId",
                table: "HrTrainingPrograms",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRecords_EmployeeId",
                table: "HrTrainingRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRecords_SchoolId",
                table: "HrTrainingRecords",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRecords_TrainingProgramId",
                table: "HrTrainingRecords",
                column: "TrainingProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRequests_AssignedTrainingProgramId",
                table: "HrTrainingRequests",
                column: "AssignedTrainingProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRequests_EmployeeId",
                table: "HrTrainingRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTrainingRequests_SchoolId",
                table: "HrTrainingRequests",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrViolationTypes_SchoolId",
                table: "HrViolationTypes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_HrWorkShifts_SchoolId",
                table: "HrWorkShifts",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Phone",
                table: "OtpVerifications",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Phone_IsUsed_CreatedAt",
                table: "OtpVerifications",
                columns: new[] { "Phone", "IsUsed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrationRequests_CreatedSchoolId",
                table: "SchoolRegistrationRequests",
                column: "CreatedSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrationRequests_Status",
                table: "SchoolRegistrationRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrationRequests_SubmittedAt",
                table: "SchoolRegistrationRequests",
                column: "SubmittedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrAdvanceDeductionLogs");

            migrationBuilder.DropTable(
                name: "HrBonuses");

            migrationBuilder.DropTable(
                name: "HrCareerHistories");

            migrationBuilder.DropTable(
                name: "HrClearanceItems");

            migrationBuilder.DropTable(
                name: "HrDailyAttendances");

            migrationBuilder.DropTable(
                name: "HrDisciplinaryActions");

            migrationBuilder.DropTable(
                name: "HrEmployeeContracts");

            migrationBuilder.DropTable(
                name: "HrEmployeeDocuments");

            migrationBuilder.DropTable(
                name: "HrEmployeeRequests");

            migrationBuilder.DropTable(
                name: "HrFingerprintRecords");

            migrationBuilder.DropTable(
                name: "HrHolidays");

            migrationBuilder.DropTable(
                name: "HrKpis");

            migrationBuilder.DropTable(
                name: "HrLeaveBalances");

            migrationBuilder.DropTable(
                name: "HrLeaveRequests");

            migrationBuilder.DropTable(
                name: "HrLoanRepaymentLogs");

            migrationBuilder.DropTable(
                name: "HrOvertimeRequests");

            migrationBuilder.DropTable(
                name: "HrPayrollItems");

            migrationBuilder.DropTable(
                name: "HrPenalties");

            migrationBuilder.DropTable(
                name: "HrPerformanceScores");

            migrationBuilder.DropTable(
                name: "HrProfessionalCertificates");

            migrationBuilder.DropTable(
                name: "HrPromotions");

            migrationBuilder.DropTable(
                name: "HrSalaryAllowances");

            migrationBuilder.DropTable(
                name: "HrSalaryDeductions");

            migrationBuilder.DropTable(
                name: "HrTrainingRecords");

            migrationBuilder.DropTable(
                name: "HrTrainingRequests");

            migrationBuilder.DropTable(
                name: "OtpVerifications");

            migrationBuilder.DropTable(
                name: "SchoolRegistrationRequests");

            migrationBuilder.DropTable(
                name: "HrSalaryAdvances");

            migrationBuilder.DropTable(
                name: "HrEndOfServices");

            migrationBuilder.DropTable(
                name: "HrViolationTypes");

            migrationBuilder.DropTable(
                name: "HrFingerprintDevices");

            migrationBuilder.DropTable(
                name: "HrLeaveTypes");

            migrationBuilder.DropTable(
                name: "HrEmployeeLoans");

            migrationBuilder.DropTable(
                name: "HrMonthlyPayrolls");

            migrationBuilder.DropTable(
                name: "HrPerformanceCriterias");

            migrationBuilder.DropTable(
                name: "HrPerformanceReviews");

            migrationBuilder.DropTable(
                name: "HrAllowanceTypes");

            migrationBuilder.DropTable(
                name: "HrDeductionTypes");

            migrationBuilder.DropTable(
                name: "HrSalaryDetails");

            migrationBuilder.DropTable(
                name: "HrTrainingPrograms");

            migrationBuilder.DropTable(
                name: "HrPerformanceCycles");

            migrationBuilder.DropTable(
                name: "HrEmployees");

            migrationBuilder.DropTable(
                name: "HrJobGradeSteps");

            migrationBuilder.DropTable(
                name: "HrJobTitles");

            migrationBuilder.DropTable(
                name: "HrWorkShifts");

            migrationBuilder.DropTable(
                name: "HrJobGrades");

            migrationBuilder.DropTable(
                name: "HrDepartments");

            migrationBuilder.DropColumn(
                name: "IncludesHrModule",
                table: "SystemSubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "HrAbsenceDeductionPerDay",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrAbsenceDeductionType",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrAutoCalculateOvertime",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrAutoDeductAbsence",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrEarlyLeaveDeductionPerMinute",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrEnableFingerprintIntegration",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrEnableSelfService",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrLateDeductionPerMinute",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrLateGracePeriodMinutes",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrMaxOvertimeHoursPerMonth",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrOvertimeRateMultiplier",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrRequireApprovalForLeaves",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrSalaryCalculationMethod",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrWorkDayEnd",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrWorkDayStart",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "HrWorkingDaysPerMonth",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsHrModuleEnabled",
                table: "Schools");
        }
    }
}
