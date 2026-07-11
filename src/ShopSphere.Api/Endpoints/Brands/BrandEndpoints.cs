using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Brands.ChangeBrandStatus;
using ShopSphere.Application.Features.Brands.CreateBrand;
using ShopSphere.Application.Features.Brands.DeleteBrand;
using ShopSphere.Application.Features.Brands.GetBrandById;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Features.Brands.UpdateBrand;
using ShopSphere.Application.Features.Categories.ChangeCategoryStatus;
using ShopSphere.Contracts.Categories;

namespace ShopSphere.Api.Endpoints.Brands;

public static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/brands")
            .RequireRateLimiting("anonymous")
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

        group.MapGet("/",
            [Authorize] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetBrandsQuery());

                return result.ToHttpResult();
            });

        group.MapGet("/{id:guid}",
            [Authorize] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetBrandByIdQuery(id));

                return result.ToHttpResult();
            });

        group.MapPut("/{id:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                UpdateBrandCommand command,
                ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest();
                }

                var result = await sender.Send(command);

                return result.ToHttpResult();
            });

        group.MapDelete("/{id:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new DeleteBrandCommand(id));

                return result.ToHttpResult();
            });

        group.MapPatch("/{id:guid}/status",
           async (
               Guid id,
               ChangeBrandStatusRequest request,
               ISender sender) =>
           {
               var result = await sender.Send(
                   new ChangeBrandStatusCommand(
                       id,
                       request.IsActive));

               return result.ToHttpResult();
           });

        return app;
    }
}