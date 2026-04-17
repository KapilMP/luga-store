using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class ResendConfigValidator : AbstractValidator<ResendConfig>
{
    public ResendConfigValidator()
    {
        RuleFor(x => x.ApiToken).NotEmpty();
    }
}
