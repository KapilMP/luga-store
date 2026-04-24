using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace SedaWears.Application.Features.Users.Commands;

public record InviteAdminCommand(string Email) : IRequest;

public class InviteAdminValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}

public class InviteAdminHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    IUserCuckooFilter cuckooFilter,
    AppConfig appConfig) : IRequestHandler<InviteAdminCommand>
{
    public async Task Handle(InviteAdminCommand request, CancellationToken ct)
    {
        User? user = null;

        if (await cuckooFilter.ExistsAsync(request.Email, UserRole.Admin))
        {
            user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin, ct);
        }

        if (user != null)
        {
            if (!user.EmailConfirmed)
                throw new BadRequestException("Email is already invited.");
            else
                throw new BadRequestException("Email is already in use.");
        }

        if (user == null)
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

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

            await emailService.SendEmailAsync(
                user.Email!,
                "SedaWears Admin Invitation",
                $"<p>You have been invited as an Admin to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>");
        }
    }
}
