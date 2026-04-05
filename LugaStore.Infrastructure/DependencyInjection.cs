using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Infrastructure.Messaging.Consumers;
using LugaStore.Infrastructure.Persistence;
using LugaStore.Infrastructure.Services;
using LugaStore.Infrastructure.Messaging;
using LugaStore.Infrastructure.Configurations;
using LugaStore.Application.Common.Configurations;
using LugaStore.Application.Common.Configurations.Validators;
using LugaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MassTransit;
using FluentValidation;

namespace LugaStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Validators
        services.AddValidatorsFromAssemblyContaining<JwtConfigValidator>();

        // Production-Grade Strategy: AddOptionsWithValidation for all Configurations
        services.AddOptionsWithValidation<ConnectionStringsConfig>(configuration, "ConnectionStrings");
        services.AddOptionsWithValidation<JwtConfig>(configuration, "Jwt");
        services.AddOptionsWithValidation<GoogleConfig>(configuration, "Google");
        services.AddOptionsWithValidation<RefreshTokenPathsConfig>(configuration, "RefreshTokenPaths");
        services.AddOptionsWithValidation<AppConfig>(configuration, "App");
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppConfig>>().Value);
        services.AddOptionsWithValidation<OpeninaryConfig>(configuration, "Openinary");
        services.AddOptionsWithValidation<EmailConfig>(configuration, "Email");
        services.AddOptionsWithValidation<RateLimitConfig>(configuration, "RateLimit");

        services.AddHttpClient<IImageService, OpeninaryService>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var connectionStrings = sp.GetRequiredService<IOptions<ConnectionStringsConfig>>().Value;
            options.UseNpgsql(
                connectionStrings.Postgres,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        // Messaging (Best Practices: Explicit naming, robust retry)
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.AddConsumer<EmailConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionStrings = context.GetRequiredService<IOptions<ConnectionStringsConfig>>().Value;
                cfg.Host(connectionStrings.RabbitMq);

                cfg.ReceiveEndpoint(MessagingConstants.EmailQueue, e =>
                {
                    e.ConfigureConsumer<EmailConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                });
            });
        });

        return services;
    }
}
