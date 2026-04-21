using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SedaWears.Application.Common.Behaviors;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Services;
using SedaWears.Application.Common.Validators;

namespace SedaWears.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();
        
        services.AddScoped<IUserService, UserService>();
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
