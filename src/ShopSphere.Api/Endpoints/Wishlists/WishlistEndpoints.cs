using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Wishlists.AddToWishlist;
using ShopSphere.Application.Features.Wishlists.GetWishlist;
using ShopSphere.Application.Features.Wishlists.MoveWishlistItemToCart;
using ShopSphere.Application.Features.Wishlists.RemoveFromWishlist;

namespace ShopSphere.Api.Endpoints.Wishlists;

public static class WishlistEndpoints
{
    public static IEndpointRouteBuilder MapWishlistEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishlist")
            .WithTags("Wishlist")
            .RequireAuthorization();

        group.MapPost("/",
            async (
                AddToWishlistCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToHttpResult();
            });
        group.MapGet("/",
            async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetWishlistQuery());

                return result.ToHttpResult();
            });

        group.MapDelete("/{productId:guid}",
            async (
                Guid productId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new RemoveFromWishlistCommand(productId));

                return result.ToHttpResult();
            });

        group.MapPost("/{productId:guid}/move-to-cart",
            async (
                Guid productId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new MoveWishlistItemToCartCommand(productId));

                return result.ToHttpResult();
            });

        return app;
    }
}