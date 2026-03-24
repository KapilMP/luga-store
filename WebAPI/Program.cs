using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using LugaStore.Application;
using LugaStore.Infrastructure;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure HyperDX / OpenTelemetry
var hyperDxConfig = builder.Configuration.GetSection("HyperDX");
var hyperDxKey = hyperDxConfig["ApiKey"];
var serviceName = hyperDxConfig["ServiceName"] ?? "LugaStore.API";

if (!string.IsNullOrEmpty(hyperDxKey))
{
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddSource(serviceName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("https://in-otel.hyperdx.io");
                opt.Headers = $"authorization={hyperDxKey}";
            }))
        .WithMetrics(metrics => metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("https://in-otel.hyperdx.io");
                opt.Headers = $"authorization={hyperDxKey}";
            }));

    builder.Logging.AddOpenTelemetry(logging =>
    {
        logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
        logging.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri("https://in-otel.hyperdx.io");
            opt.Headers = $"authorization={hyperDxKey}";
        });
    });
}

// Add services
builder.Services.AddControllers();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Jwt Secret key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Antiforgery (CSRF)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

// Register Clean Architecture layers
// AddInfrastructure now handles MassTransit, Database, and Identities
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure External Authentication (Google)
var googleSettings = builder.Configuration.GetSection("Google");
if (!string.IsNullOrEmpty(googleSettings["ClientId"]))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleSettings["ClientId"]!;
            options.ClientSecret = googleSettings["ClientSecret"]!;
            options.SignInScheme = IdentityConstants.ExternalScheme;
        });
}

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await DbSeeder.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

// Antiforgery Middleware
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
