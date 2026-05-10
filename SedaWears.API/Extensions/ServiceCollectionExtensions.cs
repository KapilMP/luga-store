using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

using SedaWears.Application;
using SedaWears.Infrastructure;
using SedaWears.Application.Common.Settings;
using SedaWears.Infrastructure.RateLimiting;
using System.Text.Json;
using System.Text.Json.Serialization;
using SedaWears.Application.Common.Interfaces;
using SedaWears.API.Services;
using SedaWears.API.Middleware;

using SedaWears.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace SedaWears.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IOriginContext, OriginContext>();
        services.AddApplication();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        // Authentication
        services.AddAuthentication(AuthConstants.BaseScheme)
        .AddPolicyScheme(AuthConstants.BaseScheme, "SedaWears Authentication", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var origin = context.Request.Headers["Origin"].ToString();
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return AuthConstants.CustomerScheme;

                var host = uri.Host.ToLower();
                if (host.StartsWith("admin.")) return AuthConstants.AdminScheme;
                if (host.StartsWith("manager.")) return AuthConstants.ManagerScheme;
                if (host.StartsWith("owner.")) return AuthConstants.OwnerScheme;

                return AuthConstants.CustomerScheme;
            };
        })
        .AddCookie(AuthConstants.AdminScheme, o => ConfigureCookie(o, AuthConstants.AdminScheme))
        .AddCookie(AuthConstants.OwnerScheme, o => ConfigureCookie(o, AuthConstants.OwnerScheme))
        .AddCookie(AuthConstants.ManagerScheme, o => ConfigureCookie(o, AuthConstants.ManagerScheme))
        .AddCookie(AuthConstants.CustomerScheme, o => ConfigureCookie(o, AuthConstants.CustomerScheme))
        .AddGoogle();

        // Configure Google Options
        services.AddOptions<GoogleOptions>(GoogleDefaults.AuthenticationScheme)
            .Configure<IOptions<GoogleConfig>>((options, configOpt) =>
            {
                var googleConfig = configOpt.Value;
                if (!string.IsNullOrEmpty(googleConfig.ClientId))
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                }
            });

        return services;
    }

    private static void ConfigureCookie(CookieAuthenticationOptions options, string cookieName)
    {
        options.Cookie.Name = cookieName;
        options.Cookie.Domain = ".sedawears.com";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitingConfig>(configuration.GetSection("RateLimiting"));
        services.AddCustomRateLimiting();
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });



        services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Disable built-in validation so that FluentValidation in MediatR is the single source of truth
        options.SuppressModelStateInvalidFilter = true;
    });

        services.AddCors();
        services.AddOptions<CorsOptions>()
            .Configure<IOptions<AppConfig>>((options, configOpt) =>
            {
                var origins = configOpt.Value.Cors.AllowedOrigins;
                options.AddPolicy("Default", policy =>
                    policy.WithOrigins([.. origins])
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });



        return services;
    }
}
