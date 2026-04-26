using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class EmailConfigValidator : AbstractValidator<EmailConfig>
{
    public EmailConfigValidator()
    {
        RuleFor(x => x.NoReplyEmail)
            .NotEmpty().WithMessage("The no-reply email address is missing. Ensure 'Email:NoReplyEmail' is set.")
            .EmailAddress().WithMessage("The 'Email:NoReplyEmail' must be a valid email address.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("The contact email address is missing. Ensure 'Email:ContactEmail' is set.")
            .EmailAddress().WithMessage("The 'Email:ContactEmail' must be a valid email address.");

        RuleFor(x => x.FromName)
            .NotEmpty().WithMessage("The sender display name is missing. Ensure 'Email:FromName' is set.");
    }
}
