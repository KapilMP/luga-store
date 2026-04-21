using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class JwtConfigValidator : AbstractValidator<JwtConfig>
{
    public JwtConfigValidator()
    {
        RuleFor(x => x.Secret)
            .NotEmpty().WithMessage("The JWT Secret is missing. Ensure 'Jwt:Secret' is set.")
            .MinimumLength(32).WithMessage("The JWT Secret must be at least 32 characters long for security.");
            
        RuleFor(x => x.Issuer)
            .NotEmpty().WithMessage("The JWT Issuer is missing. Ensure 'Jwt:Issuer' is set.");
            
        RuleFor(x => x.Audience)
            .NotEmpty().WithMessage("The JWT Audience is missing. Ensure 'Jwt:Audience' is set.");
            
        RuleFor(x => x.ExpiryInMinutes)
            .GreaterThan(0).WithMessage("The JWT token expiry must be greater than 0 minutes.");
            
        RuleFor(x => x.RefreshTokenExpiryInDays)
            .GreaterThan(0).WithMessage("The Refresh token expiry must be greater than 0 days.");
    }
}
