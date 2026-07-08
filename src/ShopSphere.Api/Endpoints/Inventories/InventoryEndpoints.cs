using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.Inventories.AdjustInventory;
using ShopSphere.Application.Features.Inventories.GetInventory;
using ShopSphere.Application.Features.Inventory.GetInventoryHistory;

namespace ShopSphere.Api.Endpoints;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/{productId:guid}/inventory")
            .WithTags("Inventory")
            .RequireAuthorization();

        group.MapGet(
                "/",
                GetInventory)
            .WithName("GetInventory")
            .WithSummary("Get inventory for a product")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost(
            "/adjust",
            AdjustInventory);

        group.MapGet(
                "/history",
                GetInventoryHistory)
            .WithName("GetInventoryHistory")
            .WithSummary("Get inventory transaction history");

        return app;
    }

    private static async Task<IResult> GetInventory(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetInventoryQuery(productId),
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> AdjustInventory(
        Guid productId,
        AdjustInventoryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AdjustInventoryCommand(
            productId,
            request.Quantity,
            request.Reason);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> GetInventoryHistory(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetInventoryHistoryQuery(productId),
            cancellationToken);

        return result.ToMinimalApiResult();
    }
}