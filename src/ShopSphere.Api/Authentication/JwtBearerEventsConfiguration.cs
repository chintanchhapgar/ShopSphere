using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Api.Authentication;

public sealed class JwtBearerEventsConfiguration 
{
    public async Task HandleChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new ApiResponse
            {
                Success = false,
                Message = "Authentication is required.",
                Errors =
                [
                    new ApiError(
                        "AUTH_UNAUTHORIZED",
                        "You must be logged in to access this resource.")
                ]
            });
    }

    public async Task HandleForbidden(
        ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new ApiResponse
            {
                Success = false,
                Message = "Access denied.",
                Errors =
                [
                    new ApiError(
                        "AUTH_FORBIDDEN",
                        "You do not have permission to perform this action.")
                ]
            });
    }
}