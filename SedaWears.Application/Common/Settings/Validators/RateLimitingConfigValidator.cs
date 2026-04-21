using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class RateLimitingConfigValidator : AbstractValidator<RateLimitingConfig>
{
    public RateLimitingConfigValidator()
    {
        RuleFor(x => x.RejectionStatusCode)
            .InclusiveBetween(400, 599).WithMessage("Rate Limiting rejection status code must be a valid HTTP error code (e.g., 429).");
            
        RuleFor(x => x.Policies)
            .NotEmpty().WithMessage("At least one Rate Limiting policy must be configured under 'RateLimiting:Policies'.");
    }
}
