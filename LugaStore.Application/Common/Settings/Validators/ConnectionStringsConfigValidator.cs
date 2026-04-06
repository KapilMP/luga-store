using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class ConnectionStringsConfigValidator : AbstractValidator<ConnectionStringsConfig>
{
    public ConnectionStringsConfigValidator()
    {
        RuleFor(x => x.Postgres).NotEmpty().WithMessage("Postgres connection string is required.")
            .Must(x => x.Contains("Host=", StringComparison.OrdinalIgnoreCase)).WithMessage("Postgres connection string must include Host.");
        RuleFor(x => x.RabbitMq)
            .NotEmpty().WithMessage("RabbitMq URL is required.")
            .Must(x => x.StartsWith("amqp://", StringComparison.OrdinalIgnoreCase) || x.StartsWith("amqps://", StringComparison.OrdinalIgnoreCase))
            .WithMessage("RabbitMq URL must start with amqp:// or amqps://");
    }
}
