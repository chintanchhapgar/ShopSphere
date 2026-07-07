using System.Text.Json;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred.",
                Errors =
                [
                    new ApiError(
                        "INTERNAL_SERVER_ERROR",
                        "Please contact support if the problem persists.")
                ]
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}