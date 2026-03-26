using FluentValidation;

namespace LugaStore.Application.Identity.Commands;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Token).NotEmpty();
        RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
    }
}
