using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Infrastructure.Persistence.Seeds;

public static class AdminSeeder
{
    public static async Task CreateAdminAsync(IServiceProvider serviceProvider, string email, string password, string firstName, string lastName)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            Console.WriteLine($"User with email {email} already exists.");
            return;
        }

        var user = new User
        {
            UserName = Guid.NewGuid().ToString(),
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
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
