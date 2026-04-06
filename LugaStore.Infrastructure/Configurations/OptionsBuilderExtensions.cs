using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FluentValidation;

namespace LugaStore.Infrastructure.Configurations;

public static class OptionsBuilderExtensions
{
    /// <summary>
    /// Binds a configuration section to a strongly-typed class, applies FluentValidation, 
    /// and ensures validation on start. Requires the validator type at compile-time.
    /// </summary>
    public static OptionsBuilder<TOptions> AddOptionsWithValidation<TOptions, TValidator>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
        where TValidator : class, IValidator<TOptions>
    {
        return services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateFluentValidation<TOptions, TValidator>()
            .ValidateOnStart();
    }

    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions, TValidator>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
        where TValidator : class, IValidator<TOptions>
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(s =>
            new FluentValidationOptions<TOptions>(s, optionsBuilder.Name));
        return optionsBuilder;
    }
}

