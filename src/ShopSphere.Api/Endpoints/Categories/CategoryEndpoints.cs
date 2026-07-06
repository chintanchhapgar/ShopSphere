using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Application.Features.Categories.CreateCategory;

namespace ShopSphere.Api.Endpoints.Categories;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
                       .WithTags("Categories");

        group.MapPost("/",
            [Authorize(Roles = "Admin")] async (
                CreateCategoryCommand command,
                ISender sender) =>
            {
                var id = await sender.Send(command);

                return Results.Created(
                    $"/api/categories/{id}",
                    new { Id = id });
            });

        return app;
    }
}