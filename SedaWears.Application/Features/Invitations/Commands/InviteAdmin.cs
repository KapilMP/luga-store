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

namespace SedaWears.Application.Features.Invitations.Commands;

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
        // 1. Check if user already exists as an Admin
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin, ct);

        if (user != null)
        {
            if (user.IsAdminInvitationAccepted == false)
                throw new BadRequestException("Email already invited.");
            else
                throw new BadRequestException("Email already in use.");
        }

        // 2. Create the new Admin user
        user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = string.Empty,
            LastName = string.Empty,
            IsActive = true,
            Role = UserRole.Admin,
            IsAdminInvitationAccepted = false
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        // 3. Update Cuckoo Filter for fast lookup
        await cuckooFilter.AddAsync(user.Email!, UserRole.Admin);

        // 4. Generate and send invitation
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}&role={nameof(UserRole.Admin)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "SedaWears Admin Invitation",
            $"<p>You have been invited as an <b>Admin</b> to the SedaWears platform.</p>" +
            $"<p>Click <a href='{url}'>here</a> to accept the invitation and set up your account password.</p>");
    }
}
