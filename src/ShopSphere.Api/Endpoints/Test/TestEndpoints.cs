using Microsoft.AspNetCore.Authorization;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Api.Endpoints.Test;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test")
            .WithTags("Test");

        group.MapPost("/email",
            [AllowAnonymous] async (
                string email,
                IEmailService emailService,
                CancellationToken cancellationToken) =>
            {
                var html = """
                    <h2>ShopSphere</h2>
                    <p>Email configuration is working successfully.</p>
                    """;

                await emailService.SendAsync(
                    email,
                    "ShopSphere Test Email",
                    html,
                    cancellationToken);

                return Results.Ok(new
                {
                    Message = "Email sent successfully."
                });
            });

        return app;
    }
}