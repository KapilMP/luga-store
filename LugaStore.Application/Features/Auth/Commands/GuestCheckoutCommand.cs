using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Auth.Commands;

public record GuestCheckoutCommand(string Email, string FirstName, string LastName, string Phone) : IRequest<bool>;

public class GuestCheckoutCommandHandler(UserManager<User> userManager) :
    IRequestHandler<GuestCheckoutCommand, bool>
{
    public async Task<bool> Handle(GuestCheckoutCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null) return true;

        var user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.Phone,
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        return true;
    }
}

public class GuestCheckoutCommandValidator : AbstractValidator<GuestCheckoutCommand>
{
    public GuestCheckoutCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}
