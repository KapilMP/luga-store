using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception occurred. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            ValidationException ex => new ValidationProblemDetails(ex.Errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },

            NotFoundError ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Resource not found",
                Detail = ex.Message,
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },

            BadRequestError ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad request",
                Detail = ex.Message,
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },

            UnauthorizedAccessException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Unauthorized",
                Detail = "You are not authenticated to access this resource.",
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            },

            ForbiddenError ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Forbidden,
                Title = "Forbidden",
                Detail = ex.Message,
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            },

            ConflictError ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflict",
                Detail = ex.Message,
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            },

            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Server error",
                Detail = env.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.",
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        context.Response.StatusCode =
            problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails,
            problemDetails.GetType(),
            JsonOptions);
    }
}