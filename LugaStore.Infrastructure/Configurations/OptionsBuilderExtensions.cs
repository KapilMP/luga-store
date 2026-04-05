using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace LugaStore.Infrastructure.Configurations;

public static class OptionsBuilderExtensions
{
    /// <summary>
    /// Binds a configuration section to a strongly-typed class, applies FluentValidation, 
    /// and ensures validation on start.
    /// </summary>
    public static OptionsBuilder<TOptions> AddOptionsWithValidation<TOptions>(
        this IServiceCollection services, 
        IConfiguration configuration, 
        string sectionName) where TOptions : class
    {
        return services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateFluentValidation()
            .ValidateOnStart();
    }

    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(s =>
            new FluentValidationOptions<TOptions>(s, optionsBuilder.Name));
        return optionsBuilder;
    }
}
