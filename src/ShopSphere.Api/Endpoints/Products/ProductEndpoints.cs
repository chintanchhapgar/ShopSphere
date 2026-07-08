using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Products.ChangeProductStatus;
using ShopSphere.Application.Features.Products.CreateProduct;
using ShopSphere.Application.Features.Products.DeleteProduct;
using ShopSphere.Application.Features.Products.GetProductById;
using ShopSphere.Application.Features.Products.GetProducts;
using ShopSphere.Application.Features.Products.UpdateProduct;
using ShopSphere.Contracts.Categories;
using ShopSphere.Contracts.Products;

namespace ShopSphere.Api.Endpoints.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapPost("/",
            [Authorize(Roles = "Admin")] async (
                CreateProductCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToCreatedHttpResult(
                    $"/api/products/{result.Value}");
            });

        group.MapGet("/",
            [Authorize] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetProductsQuery());

                return result.ToHttpResult();
            });

        group.MapGet("/{id:guid}",
            [Authorize] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetProductDetailsQuery(id));

                return result.ToHttpResult();
            });

        group.MapPut("/{id:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                UpdateProductCommand command,
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
                    new DeleteProductCommand(id));

                return result.ToHttpResult();
            });

        group.MapPatch("/{id:guid}/status",
            async (
                Guid id,
                ChangeProductStatusRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new ChangeProductStatusCommand(
                        id,
                        request.IsActive));

                return result.ToHttpResult();
            });

        return app;
    }
}