using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Seeds;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<User> userManager, 
        RoleManager<IdentityRole<int>> roleManager)
    {
        // Final Role Set for Luga Store: [Admin, Manager, Partner, User]
        string[] roleNames = { "Admin", "Manager", "Partner", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        // Seed Default Admin User
        var adminEmail = "admin@luga-store.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin"
            };

            var result = await userManager.CreateAsync(user, "LugaStore@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Seed Default Manager User
        var managerEmail = "manager@luga-store.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);

        if (managerUser == null)
        {
            var user = new User
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                FirstName = "Store",
                LastName = "Manager"
            };

            var result = await userManager.CreateAsync(user, "LugaStore@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Manager");
            }
        }
    }
}
