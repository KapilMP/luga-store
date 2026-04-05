using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LugaStore.Infrastructure.Configurations;

public class FluentValidationOptions<TOptions>(IServiceProvider serviceProvider, string? name) : IValidateOptions<TOptions> where TOptions : class
{
    public ValidateOptionsResult Validate(string? name1, TOptions options)
    {
        if (name != null && name != name1) return ValidateOptionsResult.Skip;
        ArgumentNullException.ThrowIfNull(options);

        // Resolve validator from a scope to handle any lifetime (Standard Practice)
        using var scope = serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetService<IValidator<TOptions>>();
        
        // If no validator is registered, we skip validation for this option
        if (validator == null) return ValidateOptionsResult.Success;

        var result = validator.Validate(options);
        
        if (result.IsValid) return ValidateOptionsResult.Success;

        var errors = result.Errors.Select(e => $"Configuration Validation failed for {typeof(TOptions).Name}.{e.PropertyName}: {e.ErrorMessage}");
        return ValidateOptionsResult.Fail(errors);
    }
}
