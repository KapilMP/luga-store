using FluentValidation;
using LugaStore.Application.Features.Invitations.Commands;

namespace LugaStore.Application.Features.Invitations.Validators;

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

public class InviteAdminCommandValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminCommandValidator() => RuleFor(v => v.Email).NotEmpty().EmailAddress();
}

public class ResendAdminInvitationCommandValidator : AbstractValidator<ResendAdminInvitationCommand>
{
    public ResendAdminInvitationCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class InvitePartnerCommandValidator : AbstractValidator<InvitePartnerCommand>
{
    public InvitePartnerCommandValidator() => RuleFor(v => v.Email).NotEmpty().EmailAddress();
}

public class ResendPartnerInvitationCommandValidator : AbstractValidator<ResendPartnerInvitationCommand>
{
    public ResendPartnerInvitationCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
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
    public ResendPartnerManagerInvitationCommandValidator() => RuleFor(v => v.ManagerId).GreaterThan(0);
}
