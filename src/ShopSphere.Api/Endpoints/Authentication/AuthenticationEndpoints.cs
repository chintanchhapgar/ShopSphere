using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Authentication.Login;
using ShopSphere.Application.Features.Authentication.Me;
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

        group.MapPost("/login",
            async (
                LoginCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToHttpResult();
            });

        group.MapGet("/me",
            [Authorize] async (
                ISender sender) =>
            {
                var user = await sender.Send(
                    new GetCurrentUserQuery());

                return user is null
                    ? Results.NotFound()
                    : Results.Ok(user);
            });

        return app;
    }
}