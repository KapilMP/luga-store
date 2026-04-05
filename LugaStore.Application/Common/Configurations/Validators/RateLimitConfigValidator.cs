using FluentValidation;

namespace LugaStore.Application.Common.Configurations.Validators;

public class RateLimitConfigValidator : AbstractValidator<RateLimitConfig>
{
    public RateLimitConfigValidator()
    {
        RuleFor(x => x.RejectionStatusCode).InclusiveBetween(400, 599);
        RuleFor(x => x.Policies).NotEmpty().WithMessage("This field is required");
        RuleForEach(x => x.Policies.Values).SetValidator(new RateLimitPolicyConfigValidator());
    }
}

public class RateLimitPolicyConfigValidator : AbstractValidator<RateLimitPolicyConfig>
{
    public RateLimitPolicyConfigValidator()
    {
        RuleFor(x => x.Window).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.PermitLimit).GreaterThan(0);
    }
}
