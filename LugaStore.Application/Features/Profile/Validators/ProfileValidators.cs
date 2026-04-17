using LugaStore.Application.Features.Users.Models;
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

public class GetCustomerAvatarUploadUrlCommandValidator : AbstractValidator<GetCustomerAvatarUploadUrlCommand>
{
    public GetCustomerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}

public class GetPartnerAvatarUploadUrlCommandValidator : AbstractValidator<GetPartnerAvatarUploadUrlCommand>
{
    public GetPartnerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}

public class GetPartnerManagerAvatarUploadUrlCommandValidator : AbstractValidator<GetPartnerManagerAvatarUploadUrlCommand>
{
    public GetPartnerManagerAvatarUploadUrlCommandValidator()
    {
        RuleFor(v => v.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(v => v.ContentType).NotEmpty().WithMessage("Content type is required.");
    }
}
