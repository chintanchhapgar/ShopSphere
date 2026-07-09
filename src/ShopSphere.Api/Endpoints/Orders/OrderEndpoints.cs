using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Orders.CancelOrder;
using ShopSphere.Application.Features.Orders.CreateOrder;
using ShopSphere.Application.Features.Orders.GetMyOrders;
using ShopSphere.Application.Features.Orders.GetOrderById;

namespace ShopSphere.Api.Endpoints.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/",
            [Authorize] async (
                CreateOrderCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToHttpResult();
            });

        group.MapGet("/my",
            [Authorize] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetMyOrdersQuery());

                return result.ToHttpResult();
            });

        group.MapGet("/{id:guid}",
            [Authorize] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetOrderByIdQuery(id));

                return result.ToHttpResult();
            });

        group.MapPost("/{id:guid}/cancel",
            [Authorize] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new CancelOrderCommand(id));

                return result.ToHttpResult();
            });

        return app;
    }
}