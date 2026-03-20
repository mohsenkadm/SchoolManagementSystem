using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SchoolMS.Infrastructure.Data;

/// <summary>
/// Used by EF Core CLI tools (dotnet ef migrations add/update) when no host is running.
/// Provides a SchoolDbContext with a NullTenantProvider (no tenant filtering at design time).
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SchoolDbContext>
{
    public SchoolDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SchoolMS.Web"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SchoolDbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(SchoolDbContext).Assembly.FullName));

        return new SchoolDbContext(optionsBuilder.Options, new NullTenantProvider());
    }
}

/// <summary>
/// A tenant provider that returns no tenant context.
/// Used at design time and during database seeding.
/// </summary>
public class NullTenantProvider : ITenantProvider
{
    public int? GetCurrentSchoolId() => null;
    public int? GetCurrentBranchId() => null;
    public string? GetCurrentUserId() => null;
}
