using FluentValidation;
using LugaStore.Application.Features.Profile.Commands;

namespace LugaStore.Application.Features.Profile.Validators;

public class UpdateAdminProfileCommandValidator : AbstractValidator<UpdateAdminProfileCommand>
{
    public UpdateAdminProfileCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}

public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
{
    public UpdateCustomerProfileCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}

public class UpdatePartnerProfileCommandValidator : AbstractValidator<UpdatePartnerProfileCommand>
{
    public UpdatePartnerProfileCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}

public class UploadCustomerAvatarCommandValidator : AbstractValidator<UploadCustomerAvatarCommand>
{
    public UploadCustomerAvatarCommandValidator()
    {
        RuleFor(v => v.Stream).NotNull().WithMessage("File is required.");
    }
}

public class UploadPartnerAvatarCommandValidator : AbstractValidator<UploadPartnerAvatarCommand>
{
    public UploadPartnerAvatarCommandValidator()
    {
        RuleFor(v => v.Stream).NotNull().WithMessage("File is required.");
    }
}
