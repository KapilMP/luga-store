using SedaWears.API.Middleware;

namespace SedaWears.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationPipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseExceptionHandler();

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCors("Default");
        app.UseRateLimiter();

        app.UseAuthentication();

        app.UseUserStatusValidation();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
