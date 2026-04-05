using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using LugaStore.Domain.Common;

namespace LugaStore.Infrastructure.Persistence.Seeds;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        string[] roleNames = [Roles.Admin, Roles.PartnerManager, Roles.Partner, Roles.Customer];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                Console.WriteLine($"Role {roleName} created.");
            }
        }
    }
}
