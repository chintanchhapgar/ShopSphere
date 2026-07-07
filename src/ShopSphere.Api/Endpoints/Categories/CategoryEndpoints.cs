using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Categories.ChangeCategoryStatus;
using ShopSphere.Application.Features.Categories.CreateCategory;
using ShopSphere.Application.Features.Categories.DeleteCategory;
using ShopSphere.Application.Features.Categories.GetCategories;
using ShopSphere.Application.Features.Categories.GetCategoryById;
using ShopSphere.Application.Features.Categories.UpdateCategory;
using ShopSphere.Contracts.Categories;
using ShopSphere.Domain.Constants;

namespace ShopSphere.Api.Endpoints.Categories;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        group.MapPost("/",
            async (
                CreateCategoryCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToCreatedHttpResult(
                    $"/api/categories/{result.Value}");
            });

        group.MapGet("/{id:guid}",
            async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetCategoryByIdQuery(id));

                return result.ToHttpResult();
            });

        group.MapGet("/",
            async (ISender sender) =>
            {
                var result = await sender.Send(
                    new GetCategoriesQuery());

                return result.ToHttpResult();
            });

        group.MapPut("/{id:guid}",
            async (
                Guid id,
                UpdateCategoryCommand request,
                ISender sender) =>
            {
                var command = request with { Id = id };

                var result = await sender.Send(command);

                return result.ToHttpResult();
            });

        group.MapDelete("/{id:guid}",
            async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new DeleteCategoryCommand(id));

                return result.ToHttpResult();
            });

        group.MapPatch("/{id:guid}/status",
            async (
                Guid id,
                ChangeCategoryStatusRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new ChangeCategoryStatusCommand(
                        id,
                        request.IsActive));

                return result.ToHttpResult();
            });

        return app;
    }
}