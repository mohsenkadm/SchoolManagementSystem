-- ============================================================================
-- AddMissingPermissions_StoragePlans.sql
-- 
-- Adds permissions for NEW pages that were not included in the original seed:
--   1. TeacherEarnings
--   2. StorageRequests
--   3. AcademicYears
--   4. StudentSubscriptions
--   5. OnlineSubscriptionPlans
--
-- This script:
--   1. Inserts missing Permission rows for every active school
--   2. Grants all new permissions to every non-deleted user in each school
--
-- Safe to run multiple times — uses NOT EXISTS checks everywhere.
-- ============================================================================

SET NOCOUNT ON;

BEGIN TRANSACTION;
BEGIN TRY

    -- ========================================================================
    -- STEP 1: Define the NEW pages that need permissions
    -- ========================================================================
    DECLARE @NewPages TABLE (PageName NVARCHAR(100));
    INSERT INTO @NewPages (PageName) VALUES
        ('TeacherEarnings'),
        ('StorageRequests'),
        ('AcademicYears'),
        ('StudentSubscriptions'),
        ('OnlineSubscriptionPlans');

    DECLARE @Actions TABLE (ActionName NVARCHAR(50));
    INSERT INTO @Actions (ActionName) VALUES ('View'),('Add'),('Edit'),('Delete');

    -- ========================================================================
    -- STEP 2: Insert missing Permissions for EVERY active school
    -- ========================================================================
    INSERT INTO [Permissions] ([PageName], [Action], [SchoolId], [CreatedAt], [CreatedBy], [IsDeleted])
    SELECT p.PageName, a.ActionName, s.Id, GETUTCDATE(), 'System', 0
    FROM @NewPages p
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
    -- STEP 3: Grant ALL new permissions to EVERY non-deleted user in each school
    -- ========================================================================
    INSERT INTO [UserPermissions] ([UserId], [PermissionId], [IsGranted], [SchoolId], [CreatedAt], [CreatedBy], [IsDeleted])
    SELECT u.Id, perm.Id, 1, perm.SchoolId, GETUTCDATE(), 'System', 0
    FROM [AspNetUsers] u
    INNER JOIN [Permissions] perm ON perm.SchoolId = u.SchoolId AND perm.IsDeleted = 0
    INNER JOIN @NewPages np ON np.PageName = perm.PageName
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
    PRINT '✅ Done — all new permissions seeded and granted successfully.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT CONCAT('❌ Error: ', ERROR_MESSAGE());
    THROW;
END CATCH;
