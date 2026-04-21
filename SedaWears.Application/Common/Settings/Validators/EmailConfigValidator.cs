using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class EmailConfigValidator : AbstractValidator<EmailConfig>
{
    public EmailConfigValidator()
    {
        RuleFor(x => x.FromEmail)
            .NotEmpty().WithMessage("The sender email address is missing. Ensure 'Email:FromEmail' is set.")
            .EmailAddress().WithMessage("The 'Email:FromEmail' must be a valid email address.");
            
        RuleFor(x => x.FromName)
            .NotEmpty().WithMessage("The sender display name is missing. Ensure 'Email:FromName' is set.");
    }
}
