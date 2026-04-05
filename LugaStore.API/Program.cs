using System.Text;
using System.Threading.RateLimiting;
using System.CommandLine;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LugaStore.Application;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Infrastructure;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;
using LugaStore.Infrastructure.Configurations;
using LugaStore.Application.Common.Configurations;
using LugaStore.API.Identity;

var builder = WebApplication.CreateBuilder(args);

// Register Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 1. Production-Grade Rate Limiting via IOptions<RateLimitConfig>
builder.Services.AddRateLimiter();
builder.Services.AddOptions<RateLimiterOptions>()
    .Configure<IOptions<RateLimitConfig>>((options, configOpt) =>
    {
        var config = configOpt.Value;
        options.RejectionStatusCode = config.RejectionStatusCode;

        foreach (var policy in config.Policies)
        {
            options.AddFixedWindowLimiter(policy.Key, opt =>
            {
                opt.Window = TimeSpan.Parse(policy.Value.Window);
                opt.PermitLimit = policy.Value.PermitLimit;
                opt.QueueLimit = policy.Value.QueueLimit;
                opt.QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(policy.Value.QueueProcessingOrder);
            });
        }
    });

// 2. Production-Grade CORS via IOptions<AppConfig>
builder.Services.AddCors();
builder.Services.AddOptions<CorsOptions>()
    .Configure<IOptions<AppConfig>>((options, configOpt) =>
    {
        var origins = configOpt.Value.Cors.AllowedOrigins;
        options.AddPolicy("Default", policy =>
            policy.WithOrigins([.. origins])
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
    });

// Add basic services
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Swagger
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
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

        c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
        });
    });
}

// 3. Production-Grade JWT Authentication via IOptions<JwtConfig>
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer()
.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
.Configure<IOptions<JwtConfig>>((options, configOpt) =>
{
    var jwtConfig = configOpt.Value;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Production-Grade Google Authentication via IOptions<GoogleConfig>
// Note: We use dynamic configuration for Google; if ClientId is null/empty, the scheme won't be active.
builder.Services.AddAuthentication()
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

// Configure Antiforgery (CSRF)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Register Current User Context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var app = builder.Build();

// CLI Commands
var rootCommand = new RootCommand("LugaStore Management CLI");

var seedRolesCommand = new Command("seed-roles", "Seed Roles");
seedRolesCommand.SetHandler(async () =>
{
    Console.WriteLine("--- Seeding Roles ---");
    await RoleSeeder.SeedRolesAsync(app.Services);
});
rootCommand.AddCommand(seedRolesCommand);

var createAdminCommand = new Command("create-admin", "Create an admin user");
var emailOpt = new Option<string>("--email", "Admin email");
var passwordOpt = new Option<string>("--password", "Admin password");
var firstNameOpt = new Option<string>("--first-name", "Admin first name");
var lastNameOpt = new Option<string>("--last-name", "Admin last name");

createAdminCommand.AddOption(emailOpt);
createAdminCommand.AddOption(passwordOpt);
createAdminCommand.AddOption(firstNameOpt);
createAdminCommand.AddOption(lastNameOpt);

createAdminCommand.SetHandler(async (email, password, firstName, lastName) =>
{
    Console.WriteLine("--- Create Admin User ---");

    email = Prompt("Email", email, v => !string.IsNullOrWhiteSpace(v) && v.Contains("@"));
    firstName = Prompt("First Name", firstName, v => !string.IsNullOrWhiteSpace(v));
    lastName = Prompt("Last Name", lastName, v => !string.IsNullOrWhiteSpace(v));
    password = Prompt("Password", password, v => !string.IsNullOrWhiteSpace(v), isPassword: true);

    await AdminSeeder.CreateAdminAsync(app.Services, email!, password!, firstName!, lastName!);
}, emailOpt, passwordOpt, firstNameOpt, lastNameOpt);

rootCommand.AddCommand(createAdminCommand);

if (args.Length > 0 && (args[0] == "create-admin" || args[0] == "seed-roles" || args[0] == "-h" || args[0] == "--help"))
{
    await rootCommand.InvokeAsync(args);
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(err => err.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    int status;
    object response;

    switch (ex)
    {
        case LugaStore.Application.Common.Exceptions.ValidationException e:
            status = StatusCodes.Status400BadRequest;
            response = new
            {
                title = "One or more validation errors occurred.",
                status = StatusCodes.Status400BadRequest,
                errors = e.Errors
            };
            break;
        case NotFoundError e: status = StatusCodes.Status404NotFound; response = new { error = e.Message }; break;
        case ConflictError e: status = StatusCodes.Status409Conflict; response = new { error = e.Message }; break;
        case BadRequestError e: status = StatusCodes.Status400BadRequest; response = new { error = e.Message }; break;
        case UnauthorizedError e: status = StatusCodes.Status401Unauthorized; response = new { error = e.Message }; break;
        case ForbiddenError e: status = StatusCodes.Status403Forbidden; response = new { error = e.Message }; break;
        case InternalServerError e: status = StatusCodes.Status500InternalServerError; response = new { error = e.Message }; break;
        default: status = StatusCodes.Status500InternalServerError; response = new { error = "An unexpected error occurred." }; break;
    }

    ctx.Response.StatusCode = status;
    await ctx.Response.WriteAsJsonAsync(response);
}));

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("Default");
app.UseRateLimiter();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("global");

app.Run();

// Helpers
static string Prompt(string label, string? defaultValue, Func<string, bool> validator, bool isPassword = false)
{
    if (!string.IsNullOrEmpty(defaultValue) && validator(defaultValue)) return defaultValue;

    while (true)
    {
        Console.Write($"{label}: ");
        string? input;

        if (isPassword)
        {
            input = ReadPassword();
            Console.WriteLine();
        }
        else
        {
            input = Console.ReadLine();
        }

        if (input != null && validator(input)) return input;
        Console.WriteLine($"Invalid {label}. Please try again.");
    }
}

static string ReadPassword()
{
    var pass = string.Empty;
    ConsoleKeyInfo key;
    do
    {
        key = Console.ReadKey(true);
        if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
        {
            pass += key.KeyChar;
            Console.Write("*");
        }
        else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
        {
            pass = pass[..^1];
            Console.Write("\b \b");
        }
    } while (key.Key != ConsoleKey.Enter);
    return pass;
}