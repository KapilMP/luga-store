using SedaWears.Application.Features.Users.Models;
using FluentValidation;
using SedaWears.Application.Features.Profile.Commands;

namespace SedaWears.Application.Features.Profile.Validators;

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

public class UpdateOwnerProfileCommandValidator : AbstractValidator<UpdateOwnerProfileCommand>
{
    public UpdateOwnerProfileCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}

public class GetCustomerAvatarUploadUrlCommandValidator : AbstractValidator<GetCustomerAvatarUploadUrlCommand>
{
    public GetCustomerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}

public class GetOwnerAvatarUploadUrlCommandValidator : AbstractValidator<GetOwnerAvatarUploadUrlCommand>
{
    public GetOwnerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}

public class GetManagerAvatarUploadUrlCommandValidator : AbstractValidator<GetManagerAvatarUploadUrlCommand>
{
    public GetManagerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}
