using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ShopSphere.Application.Features.Authentication.Register;

namespace ShopSphere.Api.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
                       .WithTags("Authentication");

        group.MapPost("/register",
            async (
                RegisterCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                return Results.Ok(result);
            });

        return app;
    }
}