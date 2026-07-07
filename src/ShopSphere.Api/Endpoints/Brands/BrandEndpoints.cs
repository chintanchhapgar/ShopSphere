using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Brands.CreateBrand;

namespace ShopSphere.Api.Endpoints.Brands;

public static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/brands")
            .WithTags("Brands");

        group.MapPost("/",
            [Authorize(Roles = "Admin")] async (
                CreateBrandCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToCreatedHttpResult(
                    $"/api/brands/{result.Value}");
            });

        return app;
    }
}