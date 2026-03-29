using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Infrastructure.Messaging.Consumers;
using LugaStore.Infrastructure.Persistence;
using LugaStore.Infrastructure.Services;
using LugaStore.Infrastructure.Settings;
using LugaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MassTransit;

namespace LugaStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind Configurations
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddSingleton<IJwtSettings>(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

        services.Configure<GoogleSettings>(configuration.GetSection("Google"));
        services.AddSingleton<IGoogleSettings>(sp => sp.GetRequiredService<IOptions<GoogleSettings>>().Value);

        services.Configure<RefreshTokenPaths>(configuration.GetSection("RefreshTokenPaths"));
        services.AddSingleton<IRefreshTokenPaths>(sp => sp.GetRequiredService<IOptions<RefreshTokenPaths>>().Value);

        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddSingleton<IAppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

        services.Configure<OpeninarySettings>(configuration.GetSection("Openinary"));
        services.AddSingleton<IOpeninarySettings>(sp => sp.GetRequiredService<IOptions<OpeninarySettings>>().Value);

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddSingleton<IEmailSettings>(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);

        services.Configure<RateLimitSettings>(configuration.GetSection("RateLimiting"));
        services.AddSingleton<IRateLimitSettings>(sp => sp.GetRequiredService<IOptions<RateLimitSettings>>().Value);

        services.AddHttpClient<IImageService, OpeninaryService>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPartnerService, PartnerService>();
        services.AddScoped<IEmailSender, EmailSender>();

        // Messaging
        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailConsumer>(cfg => 
            {
                cfg.UseMessageRetry(r => r.Interval(1, TimeSpan.FromSeconds(5)));
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq") ?? "localhost");
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
