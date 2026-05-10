using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace SedaWears.Application.Features.Invitations.Commands;

public record InviteAdminCommand(string Email) : IRequest;
public record ResendAdminInvitationCommand(int UserId) : IRequest;

public class InviteAdminValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}

public class AdminInvitationHandlers(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IUserService userService) :
    IRequestHandler<InviteAdminCommand>,
    IRequestHandler<ResendAdminInvitationCommand>
{
    public async Task Handle(InviteAdminCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin, ct);

        if (user != null)
        {
            if (user.IsAdminInvitationAccepted == false)
                throw new BadRequestException("Email already invited.");
            else
                throw new BadRequestException("Email already in use.");
        }

        user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = string.Empty,
            LastName = string.Empty,
            Role = UserRole.Admin,
            IsActive = false,
            IsAdminInvitationAccepted = false
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        await userService.SendInvitationEmailAsync(user);
    }

    public async Task Handle(ResendAdminInvitationCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.Role == UserRole.Admin, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.IsAdminInvitationAccepted == true)
            throw new BadRequestException("User has already accepted their invitation.");

        await userService.SendInvitationEmailAsync(user);
    }
}
