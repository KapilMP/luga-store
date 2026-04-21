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

        var existingUser = await userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            if (existingUser.IsDeleted)
            {
                Console.WriteLine($"User with email {email} exists but is soft-deleted. Please restore the user manually or use a different email.");
            }
            else
            {
                Console.WriteLine($"User with email {email} already exists and is active.");
            }
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
