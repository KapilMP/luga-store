using LugaStore.Infrastructure.Messaging.Connection;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Common.Settings.Validators;
using LugaStore.Infrastructure.Messaging.Consumers;
using LugaStore.Infrastructure.Persistence;
using LugaStore.Infrastructure.Services;
using LugaStore.Infrastructure.Messaging.Publishers;
using LugaStore.Infrastructure.ExternalServices;
using LugaStore.Infrastructure.Configurations;
using LugaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MassTransit;
using FluentValidation;

namespace LugaStore.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration for modern .NET monolith applications.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInfrastructureConfigs()
            .AddPersistence(configuration)
            .AddIdentity()
            .AddMessagingByRabbitMq()
            .AddInfrastructureServices();

        return services;
    }

    private static IServiceCollection AddInfrastructureConfigs(this IServiceCollection services)
    {
        // 1. Scan for all validators once
        services.AddValidatorsFromAssemblyContaining<JwtConfigValidator>();
        services.AddValidatorsFromAssemblyContaining<LugaStore.Application.Common.Settings.Validators.RateLimitingConfigValidator>();

        // 2. Register Configurations using BindConfiguration (Modern .NET idiomatic way)
        // This also registers the direct type (e.g. JwtConfig) as a Singleton for easier DI.
        services
            .AddConfigWithValidation<ConnectionStringsConfig, ConnectionStringsConfigValidator>("ConnectionStrings")
            .AddConfigWithValidation<JwtConfig, JwtConfigValidator>("Jwt")
            .AddConfigWithValidation<GoogleConfig, GoogleConfigValidator>("Google")
            .AddConfigWithValidation<RefreshTokenPathsConfig, RefreshTokenPathsConfigValidator>("RefreshTokenPaths")
            .AddConfigWithValidation<AppConfig, AppConfigValidator>("App")
            .AddConfigWithValidation<OpeninaryConfig, OpeninaryConfigValidator>("Openinary")
            .AddConfigWithValidation<EmailConfig, EmailConfigValidator>("Email")
            .AddConfigWithValidation<RateLimitingConfig, RateLimitingConfigValidator>("RateLimiting");

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<ConnectionStringsConfig>();
            options.UseNpgsql(config.Postgres, o =>
                o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(opt =>
      {
          opt.Password.RequireNonAlphanumeric = false;
          opt.User.RequireUniqueEmail = false;
      })
      .AddRoles<IdentityRole<int>>()
      .AddRoleManager<RoleManager<IdentityRole<int>>>()
      .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient<IImageService, OpeninaryService>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailSender, EmailPublisher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        return services;
    }

    /// <summary>
    /// Helper to register both IOptions<T> (idiomatic .NET pattern) and the raw T config class (easier injection).
    /// Uses modern .NET BindConfiguration to resolve settings from the container's IConfiguration.
    /// </summary>
    private static IServiceCollection AddConfigWithValidation<TConfig, TValidator>(this IServiceCollection services, string sectionName)
        where TConfig : class
        where TValidator : class, IValidator<TConfig>
    {
        services.AddOptionsWithValidation<TConfig, TValidator>(sectionName);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TConfig>>().Value);
        return services;
    }
}
