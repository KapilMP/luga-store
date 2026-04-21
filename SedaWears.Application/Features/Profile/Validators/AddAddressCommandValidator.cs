using FluentValidation;
using SedaWears.Application.Features.Profile.Commands;

namespace SedaWears.Application.Features.Profile.Validators;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(v => v.Label).NotEmpty();
        RuleFor(v => v.FullName).NotEmpty();
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Phone).NotEmpty();
        RuleFor(v => v.Street).NotEmpty();
        RuleFor(v => v.City).NotEmpty();
        RuleFor(v => v.ZipCode).NotEmpty();
    }
}
