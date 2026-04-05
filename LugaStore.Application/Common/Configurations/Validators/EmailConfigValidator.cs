using FluentValidation;

namespace LugaStore.Application.Common.Configurations.Validators;

public class EmailConfigValidator : AbstractValidator<EmailConfig>
{
    public EmailConfigValidator()
    {
        RuleFor(x => x.Host).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.Port).GreaterThan(0);
        RuleFor(x => x.FromEmail).EmailAddress().WithMessage("Please provide a valid email address");
        RuleFor(x => x.FromName).NotEmpty().WithMessage("This field is required");
    }
}
