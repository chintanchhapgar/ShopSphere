using System.Text.Json;
using FluentValidation;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleBadRequestException(context, ex);
        }
        catch (JsonException ex)
        {
            await HandleJsonException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleValidationException(
        HttpContext context,
        ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Message = "Validation failed.",
            Errors = ex.Errors
                .Select(x => new ApiError(
                    ErrorCodes.Validation,
                    x.ErrorMessage,
                    ToCamelCase(x.PropertyName)))
                .ToList()
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleBadRequestException(
        HttpContext context,
        BadHttpRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var jsonException = ex.InnerException as JsonException;
        var field = GetJsonField(jsonException?.Path);

        var response = new ApiResponse
        {
            Success = false,
            Message = "Validation failed.",
            Errors =
            [
                new ApiError(
                    ErrorCodes.InvalidRequest,
                    field is not null
                        ? $"The value provided for '{field}' is invalid."
                        : "Invalid JSON payload.",
                    field)
            ]
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleJsonException(
        HttpContext context,
        JsonException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var field = GetJsonField(ex.Path);

        var response = new ApiResponse
        {
            Success = false,
            Message = "Validation failed.",
            Errors =
            [
                new ApiError(
                    ErrorCodes.InvalidRequest,
                    field is not null
                        ? $"The value provided for '{field}' is invalid."
                        : "Invalid JSON payload.",
                    field)
            ]
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleException(
        HttpContext context,
        Exception ex)
    {
        _logger.LogError(ex, ex.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Message = "An unexpected error occurred.",
            Errors =
            [
                new ApiError(
                    ErrorCodes.ServerError,
                    "Please contact support if the problem persists.")
            ]
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static string? ToCamelCase(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private static string? GetJsonField(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        path = path.Trim();

        // Invalid JSON syntax (extra comma, missing brace, etc.)
        if (path == "$")
        {
            return null;
        }

        if (path.StartsWith("$."))
        {
            path = path[2..];
        }

        return string.IsNullOrWhiteSpace(path)
            ? null
            : path;
    }
}