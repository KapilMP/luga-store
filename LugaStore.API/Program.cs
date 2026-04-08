using System.CommandLine;
using LugaStore.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// CLI Commands
var rootCommand = new RootCommand("LugaStore Management CLI");
rootCommand.SetupCliCommands(app);

if (args.Length > 0 && (args[0] == "create-admin" || args[0] == "seed-roles" || args[0] == "-h" || args[0] == "--help"))
{
    await rootCommand.InvokeAsync(args);
    return;
}

// Configure Pipeline
app.UseApplicationPipeline();

app.Run();