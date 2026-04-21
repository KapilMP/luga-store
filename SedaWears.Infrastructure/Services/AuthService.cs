using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Infrastructure.Services;

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
            EmailConfirmed = true,
            Role = UserRole.Customer
        };

        var result = await userManager.CreateAsync(user);
        return result.Succeeded;
    }
}
