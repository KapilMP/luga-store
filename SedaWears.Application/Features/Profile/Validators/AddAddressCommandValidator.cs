using FluentValidation;
using SedaWears.Application.Features.Profile.Commands;

namespace SedaWears.Application.Features.Profile.Validators;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(v => v.Label).NotEmpty().WithMessage("Label is required.");
        RuleFor(v => v.FullName).NotEmpty().WithMessage("Full name is required.");
        RuleFor(v => v.Email).NotEmpty().WithMessage("Email address is required.").EmailAddress().WithMessage("Please provide a valid email address.");
        RuleFor(v => v.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(v => v.Street).NotEmpty().WithMessage("Street address is required.");
        RuleFor(v => v.City).NotEmpty().WithMessage("City is required.");
        RuleFor(v => v.ZipCode).NotEmpty().WithMessage("Zip code is required.");
    }
}
