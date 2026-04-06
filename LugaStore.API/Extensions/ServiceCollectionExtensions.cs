using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;
using LugaStore.Application;
using LugaStore.Infrastructure;
using LugaStore.Infrastructure.Configurations;
using LugaStore.Application.Common.Settings;
using LugaStore.Infrastructure.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
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
        .AddJwtBearer();
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
            .ConfigureApiBehaviorOptions(options =>
            {
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

        if (environment.IsDevelopment())
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Luga Store API", Version = "v1" });
                c.CustomSchemaIds(type => type.FullName!.Replace("+", ".").Replace("[", "_").Replace("]", "_"));

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });

                // Using doc delegate and project-specific reference for compatibility
                c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
                });
            });
        }

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        return services;
    }
}
