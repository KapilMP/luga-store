using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class EmailConfigValidator : AbstractValidator<EmailConfig>
{
    public EmailConfigValidator()
    {
        RuleFor(x => x.FromName).NotEmpty();
        RuleFor(x => x.FromEmail).NotEmpty().EmailAddress();
    }
}
