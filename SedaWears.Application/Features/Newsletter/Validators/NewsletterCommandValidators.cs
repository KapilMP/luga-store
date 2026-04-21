using FluentValidation;
using SedaWears.Application.Features.Newsletter.Commands;

namespace SedaWears.Application.Features.Newsletter.Validators;

public class SubscribeCommandValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
    }
}

public class UnsubscribeCommandValidator : AbstractValidator<UnsubscribeCommand>
{
    public UnsubscribeCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
    }
}

public class ConfirmUnsubscribeCommandValidator : AbstractValidator<ConfirmUnsubscribeCommand>
{
    public ConfirmUnsubscribeCommandValidator()
    {
        RuleFor(v => v.Token).NotEmpty();
    }
}
