using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class ResendConfigValidator : AbstractValidator<ResendConfig>
{
    public ResendConfigValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("The Resend API Key is missing. Ensure 'Resend:ApiKey' is configured for email services.");
    }
}
