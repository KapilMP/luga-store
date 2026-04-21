using System.CommandLine;
using SedaWears.API.Extensions;
using SedaWears.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// Seed Redis Cuckoo Filters
await CuckooFilterSeeder.SeedAsync(app.Services);

// CLI Commands
var rootCommand = new RootCommand("SedaWears Management CLI");
rootCommand.SetupCliCommands(app);

if (args.Length > 0 && (args[0] == "create-admin" || args[0] == "-h" || args[0] == "--help"))
{
    await rootCommand.InvokeAsync(args);
    return;
}

// Configure Pipeline
app.UseApplicationPipeline();

app.Run();