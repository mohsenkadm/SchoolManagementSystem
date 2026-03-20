using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SchoolMS.Infrastructure.Data;
using SchoolMS.Infrastructure.Repositories;
using SchoolMS.Infrastructure.Services;

namespace SchoolMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SchoolDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SchoolDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<SchoolDbContext>()
        .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure services
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<ISubscriptionLimitService, SubscriptionLimitService>();
        services.AddScoped<IStorageQuotaService, StorageQuotaService>();

        // Background services
        services.AddHostedService<ScheduledVideoPublisher>();

        // External services
        services.AddHttpClient();
        services.AddSingleton<IBunnyStreamService, BunnyStreamService>();
        services.AddSingleton<ICloudFlareLiveService, CloudFlareLiveService>();
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}
