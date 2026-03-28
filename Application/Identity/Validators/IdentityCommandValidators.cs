using FluentValidation;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Identity.Commands;

namespace LugaStore.Application.Identity.Validators;

public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(v => v.IdToken).NotEmpty().WithMessage("Google ID Token is required.");
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
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

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Token).NotEmpty();
        RuleFor(v => v.NewPassword).NotEmpty().MinimumLength(8);
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.CurrentPassword).NotEmpty();
        RuleFor(v => v.NewPassword).NotEmpty().MinimumLength(8);
    }
}

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Token).NotEmpty();
        RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
    }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand<BaseUserProfile>>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}

public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand<BaseUserProfile>>
{
    public UploadAvatarCommandValidator()
    {
        RuleFor(v => v.Stream).NotNull().WithMessage("File is required.");
    }
}

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.Token).NotEmpty();
    }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken).NotEmpty();
    }
}

// Admin Identity Validators
public class InviteAdminCommandValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminCommandValidator() => RuleFor(v => v.Email).NotEmpty().EmailAddress();
}

public class ResendAdminInvitationCommandValidator : AbstractValidator<ResendAdminInvitationCommand>
{
    public ResendAdminInvitationCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class ActivateAdminCommandValidator : AbstractValidator<ActivateAdminCommand>
{
    public ActivateAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class DeactivateAdminCommandValidator : AbstractValidator<DeactivateAdminCommand>
{
    public DeactivateAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class DeleteAdminCommandValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

// Partner Admin Validators
public class InvitePartnerCommandValidator : AbstractValidator<InvitePartnerCommand>
{
    public InvitePartnerCommandValidator() => RuleFor(v => v.Email).NotEmpty().EmailAddress();
}

public class ResendPartnerInvitationCommandValidator : AbstractValidator<ResendPartnerInvitationCommand>
{
    public ResendPartnerInvitationCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class DeletePartnerCommandValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class ActivatePartnerCommandValidator : AbstractValidator<ActivatePartnerCommand>
{
    public ActivatePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class DeactivatePartnerCommandValidator : AbstractValidator<DeactivatePartnerCommand>
{
    public DeactivatePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class InvitePartnerManagerCommandValidator : AbstractValidator<InvitePartnerManagerCommand>
{
    public InvitePartnerManagerCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
    }
}

public class ResendPartnerManagerInvitationCommandValidator : AbstractValidator<ResendPartnerManagerInvitationCommand>
{
    public ResendPartnerManagerInvitationCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class ActivatePartnerManagerCommandValidator : AbstractValidator<ActivatePartnerManagerCommand>
{
    public ActivatePartnerManagerCommandValidator() 
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class DeactivatePartnerManagerCommandValidator : AbstractValidator<DeactivatePartnerManagerCommand>
{
    public DeactivatePartnerManagerCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class DeletePartnerManagerCommandValidator : AbstractValidator<DeletePartnerManagerCommand>
{
    public DeletePartnerManagerCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

// Partner Context Validators
public class InviteManagerCommandValidator : AbstractValidator<InviteManagerCommand>
{
    public InviteManagerCommandValidator() => RuleFor(v => v.Email).NotEmpty().EmailAddress();
}

public class ResendManagerInvitationCommandValidator : AbstractValidator<ResendManagerInvitationCommand>
{
    public ResendManagerInvitationCommandValidator() => RuleFor(v => v.ManagerId).GreaterThan(0);
}

public class ActivateManagerCommandValidator : AbstractValidator<ActivateManagerCommand>
{
    public ActivateManagerCommandValidator() => RuleFor(v => v.ManagerId).GreaterThan(0);
}

public class DeactivateManagerCommandValidator : AbstractValidator<DeactivateManagerCommand>
{
    public DeactivateManagerCommandValidator() => RuleFor(v => v.ManagerId).GreaterThan(0);
}

public class DeleteManagerCommandValidator : AbstractValidator<DeleteManagerCommand>
{
    public DeleteManagerCommandValidator() => RuleFor(v => v.ManagerId).GreaterThan(0);
}
