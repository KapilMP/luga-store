using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class GoogleConfigValidator : AbstractValidator<GoogleConfig>
{
    public GoogleConfigValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty().WithMessage("This field is required. Google authentication will not work correctly without a valid ClientId.");
        RuleFor(x => x.ClientSecret).NotEmpty().WithMessage("This field is required.");
    }
}
