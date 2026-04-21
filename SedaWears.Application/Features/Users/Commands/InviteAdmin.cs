using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using FluentValidation;

namespace SedaWears.Application.Features.Users.Commands;

public record InviteAdminCommand(string Email) : IRequest;

public class InviteAdminValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Enter valid email address").EmailAddress().WithMessage("Enter valid email address");
    }
}

public class InviteAdminHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IUserCuckooFilter cuckooFilter,
    AppConfig appConfig) : IRequestHandler<InviteAdminCommand>
{
    public async Task Handle(InviteAdminCommand request, CancellationToken ct)
    {
        User? user = null;

        // Optimized check: If definitely not in Admin filter, we skip the initial "find by email" 
        // if we are sure we only care about existing admins. 
        // However, since Identity requires unique emails globally, we still need to check if the user exists at all.
        // But if they are an admin, they will BE in this filter.
        if (await cuckooFilter.ExistsAsync(request.Email, UserRole.Admin))
        {
            user = await userManager.FindByEmailAsync(request.Email);
        }

        if (user != null && user.Role == UserRole.Admin)
        {
            if (!user.EmailConfirmed)
            {
                throw new BadRequestException("User is already Invited.");
            }
            else
            {
                throw new BadRequestException("User is already an Admin.");
            }
        }
        else
        {
            user = new User
            {
                Email = request.Email,
                UserName = Guid.NewGuid().ToString(),
                FirstName = "",
                LastName = "",
                IsActive = true,
                Role = UserRole.Admin
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            await cuckooFilter.AddAsync(user.Email!, UserRole.Admin);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "SedaWears Admin Invitation",
            $"<p>You have been invited as an Admin to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>");
    }
}
