using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Orders.Admin.GetOrders;
using ShopSphere.Application.Features.Orders.Admin.UpdateOrderStatus;
using ShopSphere.Contracts.Orders;

namespace ShopSphere.Api.Endpoints.Orders;

public static class AdminOrderEndpoints
{
    public static IEndpointRouteBuilder MapAdminOrderEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/orders")
            .WithTags("Admin Orders");

        group.MapGet("/",
            [Authorize(Roles = "Admin")] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetOrdersQuery());

                return result.ToHttpResult();
            });

        group.MapPatch("/{id:guid}/status",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                UpdateOrderStatusRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new UpdateOrderStatusCommand(
                        id,
                        request.Status));

                return result.ToHttpResult();
            });

        return app;
    }
}