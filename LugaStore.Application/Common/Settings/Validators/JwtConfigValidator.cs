using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class JwtConfigValidator : AbstractValidator<JwtConfig>
{
    public JwtConfigValidator()
    {
        RuleFor(x => x.Secret).NotEmpty().WithMessage("This field is required").MinimumLength(32);
        RuleFor(x => x.Issuer).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.Audience).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.ExpiryInMinutes).GreaterThan(0);
        RuleFor(x => x.RefreshTokenExpiryInDays).GreaterThan(0);
    }
}
