using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using LugaStore.Application;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Infrastructure;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("global", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add services
builder.Services.AddControllers();

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
    options.HeaderName = "C-CSRF-TOKEN";
});

// Register Clean Architecture layers
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(err => err.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    int status;
    string message;
    switch (ex)
    {
        case NotFoundError e: status = StatusCodes.Status404NotFound; message = e.Message; break;
        case ConflictError e: status = StatusCodes.Status409Conflict; message = e.Message; break;
        case BadRequestError e: status = StatusCodes.Status400BadRequest; message = e.Message; break;
        case UnauthorizedError e: status = StatusCodes.Status401Unauthorized; message = e.Message; break;
        case ForbiddenError e: status = StatusCodes.Status403Forbidden; message = e.Message; break;
        case InternalServerError e: status = StatusCodes.Status500InternalServerError; message = e.Message; break;
        default: status = StatusCodes.Status500InternalServerError; message = "An unexpected error occurred."; break;
    }
    ctx.Response.StatusCode = status;
    await ctx.Response.WriteAsJsonAsync(new { error = message });
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