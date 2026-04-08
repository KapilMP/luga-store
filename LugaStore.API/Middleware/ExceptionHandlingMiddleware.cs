using System.Net;
using System.Text.Json;
using LugaStore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException ex => (
                (int)HttpStatusCode.BadRequest,
                "Validation Error",
                "One or more validation errors occurred.",
                (object?)ex.Errors),
            NotFoundError ex => (
                (int)HttpStatusCode.NotFound,
                "Not Found",
                ex.Message,
                null),
            BadRequestError ex => (
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                ex.Message,
                null),
            UnauthorizedError ex => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized",
                ex.Message,
                null),
            ForbiddenError ex => (
                (int)HttpStatusCode.Forbidden,
                "Forbidden",
                ex.Message,
                null),
            ConflictError ex => (
                (int)HttpStatusCode.Conflict,
                "Conflict",
                ex.Message,
                null),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred on the server.",
                null)
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors != null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        var result = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(result);
    }
}
