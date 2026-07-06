using MediatR;
using ShopSphere.Application.Authentication.Register;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegisterCommand command,
            ISender sender) =>
        {
            var id = await sender.Send(command);

            return Results.Created(
                $"/api/users/{id}",
                id);
        });

        return app;
    }
}