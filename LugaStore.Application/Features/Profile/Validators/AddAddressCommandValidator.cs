using FluentValidation;
using LugaStore.Application.Features.Profile.Commands;

namespace LugaStore.Application.Features.Profile.Validators;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(v => v.Address.Label).NotEmpty();
        RuleFor(v => v.Address.FullName).NotEmpty();
        RuleFor(v => v.Address.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Address.Phone).NotEmpty();
        RuleFor(v => v.Address.Street).NotEmpty();
        RuleFor(v => v.Address.City).NotEmpty();
        RuleFor(v => v.Address.ZipCode).NotEmpty();
    }
}
