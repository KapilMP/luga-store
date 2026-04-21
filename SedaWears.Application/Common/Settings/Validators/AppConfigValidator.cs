using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class AppConfigValidator : AbstractValidator<AppConfig>
{
    public AppConfigValidator()
    {
        RuleFor(x => x.FrontendUrl)
            .NotEmpty().WithMessage("The Frontend URL is missing. Ensure 'AppConfig:FrontendUrl' is configured.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("The Frontend URL must be a valid absolute URI (e.g., https://example.com).");
    }
}
