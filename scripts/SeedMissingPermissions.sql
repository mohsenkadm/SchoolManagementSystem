-- ============================================================================
-- SeedMissingPermissions.sql
-- 
-- This script:
--   1. Inserts any missing Permission rows (PageName + Action) for every school
--   2. Grants all missing permissions to every non-deleted user in every school
--
-- Safe to run multiple times — uses NOT EXISTS checks everywhere.
-- ============================================================================

SET NOCOUNT ON;

BEGIN TRANSACTION;
BEGIN TRY

    -- ========================================================================
    -- STEP 1: Define the complete set of pages & actions used in controllers
    -- ========================================================================
    DECLARE @Pages TABLE (PageName NVARCHAR(100));
    INSERT INTO @Pages (PageName) VALUES
        -- Core / Original
        ('Dashboard'),('Students'),('Teachers'),('Staff'),('Subjects'),('Divisions'),('Grades'),('ClassRooms'),
        ('TeacherAssignments'),('Salaries'),('Expenses'),('Installments'),('ExamSchedule'),('WeeklySchedule'),
        ('StudentGrades'),('Branches'),('Leaves'),('Attendance'),('Notifications'),('Users'),('Promotion'),
        ('Homework'),('Quizzes'),('Carousel'),('OnlinePlans'),('OnlineSubscriptions'),('PromoCodes'),
        ('Courses'),('LiveStreams'),('Chat'),
        -- Feature Modules
        ('Parents'),('Events'),('Announcements'),('Behavior'),('Health'),('Complaints'),
        ('Visitors'),('Library'),('Transport'),('Assets'),('AuditLog'),('Reports'),
        -- New Modules
        ('TeacherEarnings'),('StorageRequests'),('AcademicYears'),('StudentSubscriptions'),('OnlineSubscriptionPlans'),
        -- HR Module
        ('HrDashboard'),('HrDepartments'),('HrJobTitles'),('HrJobGrades'),('HrEmployees'),
        ('HrContracts'),('HrWorkShifts'),('HrFingerprint'),('HrAttendance'),('HrOvertime'),
        ('HrSalary'),('HrAdvances'),('HrLoans'),('HrBonuses'),('HrPenalties'),('HrPayroll'),
        ('HrPromotions'),('HrLeaves'),('HrLeaveTypes'),('HrHolidays'),
        ('HrPerformance'),('HrTraining'),('HrDisciplinary'),('HrEndOfService'),('HrSettings');

    DECLARE @Actions TABLE (ActionName NVARCHAR(50));
    INSERT INTO @Actions (ActionName) VALUES ('View'),('Add'),('Edit'),('Delete');

    -- ========================================================================
    -- STEP 2: Insert missing Permissions for EVERY school
    -- ========================================================================
    INSERT INTO [Permissions] ([PageName], [Action], [SchoolId], [CreatedAt], [CreatedBy], [IsDeleted])
    SELECT p.PageName, a.ActionName, s.Id, GETUTCDATE(), 'System', 0
    FROM @Pages p
    CROSS JOIN @Actions a
    CROSS JOIN [Schools] s
    WHERE s.IsDeleted = 0
      AND NOT EXISTS (
          SELECT 1 FROM [Permissions] perm
          WHERE perm.PageName   = p.PageName
            AND perm.[Action]   = a.ActionName
            AND perm.SchoolId   = s.Id
            AND perm.IsDeleted  = 0
      );

    DECLARE @PermissionsInserted INT = @@ROWCOUNT;
    PRINT CONCAT('Permissions inserted: ', @PermissionsInserted);

    -- ========================================================================
    -- STEP 3: Grant ALL permissions to EVERY non-deleted user in each school
    -- ========================================================================
    INSERT INTO [UserPermissions] ([UserId], [PermissionId], [IsGranted], [SchoolId], [CreatedAt], [CreatedBy], [IsDeleted])
    SELECT u.Id, perm.Id, 1, perm.SchoolId, GETUTCDATE(), 'System', 0
    FROM [AspNetUsers] u
    INNER JOIN [Permissions] perm ON perm.SchoolId = u.SchoolId AND perm.IsDeleted = 0
    WHERE u.IsDeleted = 0
      AND NOT EXISTS (
          SELECT 1 FROM [UserPermissions] up
          WHERE up.UserId       = u.Id
            AND up.PermissionId = perm.Id
            AND up.IsDeleted    = 0
      );

    DECLARE @UserPermissionsInserted INT = @@ROWCOUNT;
    PRINT CONCAT('UserPermissions granted: ', @UserPermissionsInserted);

    COMMIT TRANSACTION;
    PRINT '✅ Done — all permissions seeded and granted successfully.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT CONCAT('❌ Error: ', ERROR_MESSAGE());
    THROW;
END CATCH;
