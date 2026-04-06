using System.CommandLine;
using LugaStore.API.Extensions;
using LugaStore.Infrastructure.Persistence.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Register Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// CLI Commands (optional: could be moved to an extension too)
var rootCommand = new RootCommand("LugaStore Management CLI");
SetupCliCommands(rootCommand, app);

if (args.Length > 0 && (args[0] == "create-admin" || args[0] == "seed-roles" || args[0] == "-h" || args[0] == "--help"))
{
    await rootCommand.InvokeAsync(args);
    return;
}

// Configure Pipeline
app.UseApplicationPipeline();

app.Run();

// Helpers for CLI
static void SetupCliCommands(RootCommand rootCommand, WebApplication app)
{
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
}

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
        else input = Console.ReadLine();

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