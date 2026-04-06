using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using LugaStore.Application.Common.Exceptions;
using Microsoft.Extensions.Hosting;

namespace LugaStore.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(err => err.Run(async ctx =>
        {
            var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
            int status;
            object response;

            switch (ex)
            {
                case FluentValidation.ValidationException e:
                    status = StatusCodes.Status400BadRequest;
                    response = new
                    {
                        title = "One or more validation errors occurred.",
                        status = StatusCodes.Status400BadRequest,
                        errors = e.Errors
                    };
                    break;
                case NotFoundError e: status = StatusCodes.Status404NotFound; response = new { error = e.Message }; break;
                case ConflictError e: status = StatusCodes.Status409Conflict; response = new { error = e.Message }; break;
                case BadRequestError e: status = StatusCodes.Status400BadRequest; response = new { error = e.Message }; break;
                case UnauthorizedError e: status = StatusCodes.Status401Unauthorized; response = new { error = e.Message }; break;
                case ForbiddenError e: status = StatusCodes.Status403Forbidden; response = new { error = e.Message }; break;
                case InternalServerError e: status = StatusCodes.Status500InternalServerError; response = new { error = e.Message }; break;
                default: status = StatusCodes.Status500InternalServerError; response = new { error = "An unexpected error occurred." }; break;
            }

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(response);
        }));

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCors("Default");
        app.UseRateLimiter();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireRateLimiting("Global");

        return app;
    }
}
