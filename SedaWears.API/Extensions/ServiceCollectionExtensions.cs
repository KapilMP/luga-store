using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;

using SedaWears.Application;
using SedaWears.Infrastructure;
using SedaWears.Infrastructure.Configurations;
using SedaWears.Application.Common.Settings;
using SedaWears.Infrastructure.RateLimiting;
using System.Text.Json;
using System.Text.Json.Serialization;
using SedaWears.Application.Common.Interfaces;
using SedaWears.API.Services;

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

        // JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
        });
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        // Google
        services.AddAuthentication()
        .AddGoogle()
        .Services.AddOptions<GoogleOptions>(GoogleDefaults.AuthenticationScheme)
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

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitingConfig>(configuration.GetSection("RateLimiting"));
        services.AddCustomRateLimiting();
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
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



        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        return services;
    }
}
