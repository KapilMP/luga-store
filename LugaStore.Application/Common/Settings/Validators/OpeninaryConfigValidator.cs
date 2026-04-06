using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class OpeninaryConfigValidator : AbstractValidator<OpeninaryConfig>
{
    public OpeninaryConfigValidator()
    {
        RuleFor(x => x.BaseUrl).NotEmpty().WithMessage("This field is required").Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _));
        RuleFor(x => x.ApiKey).NotEmpty().WithMessage("This field is required");
    }
}
