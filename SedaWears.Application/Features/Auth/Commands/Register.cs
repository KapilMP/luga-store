using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using SedaWears.Application.Common.Validators;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone) : IRequest;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .Password();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.");
    }
}

public class RegisterHandler(
    UserManager<User> userManager) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.Phone,
            Role = UserRole.Customer
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
    }
}
