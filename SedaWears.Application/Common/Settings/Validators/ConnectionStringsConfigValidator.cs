using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class ConnectionStringsConfigValidator : AbstractValidator<ConnectionStringsConfig>
{
    public ConnectionStringsConfigValidator()
    {
        RuleFor(x => x.Postgres)
            .NotEmpty().WithMessage("The database connection string is missing. Ensure 'ConnectionStrings:Postgres' is configured.");

        RuleFor(x => x.Redis)
            .NotEmpty().WithMessage("The Redis connection string is missing. Ensure 'ConnectionStrings:Redis' is configured.");
    }
}
