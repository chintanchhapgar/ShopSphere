using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.Carts.RemoveCoupon;
using ShopSphere.Application.Features.Carts.AddCartItem;
using ShopSphere.Application.Features.Carts.ApplyCoupon;
using ShopSphere.Application.Features.Carts.ClearCart;
using ShopSphere.Application.Features.Carts.GetCart;
using ShopSphere.Application.Features.Carts.RemoveCartItem;
using ShopSphere.Application.Features.Carts.UpdateCartItem;

namespace ShopSphere.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart")
            .WithTags("Cart")
            .RequireRateLimiting("authenticated")
            .RequireAuthorization();

        group.MapGet(
                "/",
                GetCart)
            .WithName("GetCart")
            .WithSummary("Get the current user's shopping cart")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost(
                "/items",
                AddCartItem)
            .WithName("AddCartItem")
            .WithSummary("Add an item to the shopping cart")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut(
                "/items/{itemId:guid}",
                UpdateCartItem)
            .WithName("UpdateCartItem")
            .WithSummary("Update cart item quantity");

        group.MapDelete(
                "/items/{itemId:guid}",
                RemoveCartItem)
            .WithName("RemoveCartItem")
            .WithSummary("Remove item from shopping cart");

        group.MapDelete(
                "/",
                ClearCart)
            .WithName("ClearCart")
            .WithSummary("Clear the shopping cart");

        group.MapPost("/coupon",
            [Authorize] async (
                ApplyCouponCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToHttpResult();
            });

        group.MapDelete("/coupon",
            [Authorize] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new RemoveCouponCommand());

                return result.ToHttpResult();
            });

        return app;
    }

    private static async Task<IResult> GetCart(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCartQuery(),
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> AddCartItem(
        AddCartItemRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddCartItemCommand(
            request.ProductId,
            request.Quantity);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UpdateCartItem(
        Guid itemId,
        UpdateCartItemRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand(
            itemId,
            request.Quantity);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> RemoveCartItem(
        Guid itemId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveCartItemCommand(itemId),
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> ClearCart(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ClearCartCommand(),
            cancellationToken);

        return result.ToMinimalApiResult();
    }
}