using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Infrastructure.Persistence.Seeds;

public static class AdminSeeder
{
    public static async Task CreateAdminAsync(IServiceProvider serviceProvider, string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));

        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var existingAdmin = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Role == UserRole.Admin);

        if (existingAdmin != null)
        {
            return;
        }

        var user = new User
        {
            UserName = Guid.NewGuid().ToString(),
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            Role = UserRole.Admin
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            Console.WriteLine($"Admin user {email} created successfully.");
        }
        else
        {
            Console.WriteLine("Failed to create admin user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
}
