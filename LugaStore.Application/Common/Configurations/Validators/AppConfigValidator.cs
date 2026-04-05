using FluentValidation;

namespace LugaStore.Application.Common.Configurations.Validators;

public class AppConfigValidator : AbstractValidator<AppConfig>
{
    public AppConfigValidator()
    {
        RuleFor(x => x.FrontendUrl)
            .NotEmpty().WithMessage("This field is required")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("FrontendUrl must be a valid absolute URI.");
    }
}
