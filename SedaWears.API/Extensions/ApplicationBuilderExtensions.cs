using SedaWears.API.Middleware;

namespace SedaWears.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCors("Default");
        app.UseRateLimiter();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ShopContextMiddleware>();

        app.MapControllers();

        return app;
    }
}
