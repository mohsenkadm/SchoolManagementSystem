-- ============================================================================
-- Add missing Permission pages to all existing schools and grant them
-- to all SchoolAdmin users.
--
-- Missing pages:
--   AcademicYears, OnlineSubscriptionPlans, StudentSubscriptions,
--   TeacherEarnings, SchoolSubscriptions
--
-- Run against: SchoolMS database (SQL Server / LocalDB)
-- ============================================================================

SET NOCOUNT ON;

DECLARE @Now DATETIME2 = SYSUTCDATETIME();

-- ── 1. New permission page names that are missing ──────────────────────
DECLARE @NewPages TABLE (PageName NVARCHAR(100));
INSERT INTO @NewPages (PageName) VALUES
    ('AcademicYears'),
    ('OnlineSubscriptionPlans'),
    ('StudentSubscriptions'),
    ('TeacherEarnings'),
    ('SchoolSubscriptions');

DECLARE @Actions TABLE (ActionName NVARCHAR(50));
INSERT INTO @Actions (ActionName) VALUES ('View'), ('Add'), ('Edit'), ('Delete');

-- ── 2. Insert permissions for every school × page × action (skip existing) ─
INSERT INTO Permissions (PageName, [Action], SchoolId, CreatedAt, CreatedBy, IsDeleted)
SELECT np.PageName, a.ActionName, s.Id, @Now, 'Migration', 0
FROM Schools s
CROSS JOIN @NewPages np
CROSS JOIN @Actions a
WHERE s.IsDeleted = 0
  AND NOT EXISTS (
      SELECT 1 FROM Permissions p
      WHERE p.SchoolId = s.Id
        AND p.PageName = np.PageName
        AND p.[Action] = a.ActionName
        AND p.IsDeleted = 0
  );

PRINT 'Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' new permission rows.';

-- ── 3. Grant the new permissions to all SchoolAdmin users ──────────────
INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, SchoolId, CreatedAt, CreatedBy, IsDeleted)
SELECT ur.UserId, p.Id, 1, p.SchoolId, @Now, 'Migration', 0
FROM AspNetUserRoles ur
INNER JOIN AspNetRoles r ON r.Id = ur.RoleId AND r.Name = 'SchoolAdmin'
INNER JOIN Permissions p ON p.SchoolId IN (
    -- Get the SchoolId for each admin user from the AspNetUsers table
    SELECT u.SchoolId FROM AspNetUsers u WHERE u.Id = ur.UserId
)
INNER JOIN @NewPages np ON np.PageName = p.PageName
INNER JOIN @Actions a ON a.ActionName = p.[Action]
WHERE p.IsDeleted = 0
  AND p.PageName = np.PageName
  AND p.[Action] = a.ActionName
  AND NOT EXISTS (
      SELECT 1 FROM UserPermissions up
      WHERE up.UserId = ur.UserId
        AND up.PermissionId = p.Id
        AND up.IsDeleted = 0
  );

PRINT 'Granted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' new user-permission rows to SchoolAdmin users.';

-- ── 4. Also grant to SuperAdmin users (if any have explicit permissions) ─
INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, SchoolId, CreatedAt, CreatedBy, IsDeleted)
SELECT ur.UserId, p.Id, 1, p.SchoolId, @Now, 'Migration', 0
FROM AspNetUserRoles ur
INNER JOIN AspNetRoles r ON r.Id = ur.RoleId AND r.Name = 'SuperAdmin'
INNER JOIN Permissions p ON 1=1
INNER JOIN @NewPages np ON np.PageName = p.PageName
INNER JOIN @Actions a ON a.ActionName = p.[Action]
WHERE p.IsDeleted = 0
  AND p.PageName = np.PageName
  AND p.[Action] = a.ActionName
  AND NOT EXISTS (
      SELECT 1 FROM UserPermissions up
      WHERE up.UserId = ur.UserId
        AND up.PermissionId = p.Id
        AND up.IsDeleted = 0
  );

PRINT 'Granted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' new user-permission rows to SuperAdmin users.';
PRINT 'Done!';
