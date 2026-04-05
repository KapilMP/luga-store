using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Infrastructure.Services;

public class AuthService(UserManager<User> userManager) : IAuthService
{
    public async Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null) return true; // Already exists
        
        user = new User
        {
            Email = email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded) return false;
        await userManager.AddToRoleAsync(user, Roles.Customer);
        return true;
    }
}
