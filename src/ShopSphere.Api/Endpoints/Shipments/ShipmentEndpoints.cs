using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Shipments.CreateShipment;
using ShopSphere.Application.Features.Shipments.GetMyShipments;
using ShopSphere.Application.Features.Shipments.GetShipment;
using ShopSphere.Application.Features.Shipments.GetShipmentByOrder;
using ShopSphere.Application.Features.Shipments.UpdateShipmentStatus;

namespace ShopSphere.Api.Endpoints.Shipments;

public static class ShipmentEndpoints
{
    public static IEndpointRouteBuilder MapShipmentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shipments")
            .RequireRateLimiting("authenticated")
            .WithTags("Shipments");

        group.MapPost("/order/{orderId:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid orderId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new CreateShipmentCommand(orderId));

                return result.ToCreatedHttpResult(
                    $"/api/shipments/{result.Value}");
            });

        group.MapPatch("/{shipmentId:guid}/status",
            [Authorize(Roles = "Admin")] async (
                Guid shipmentId,
                UpdateShipmentStatusRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new UpdateShipmentStatusCommand(
                        shipmentId,
                        request.Status,
                        request.TrackingNumber,
                        request.Carrier));

                return result.ToHttpResult();
            });

        group.MapGet("/{shipmentId:guid}",
            [Authorize] async (
                Guid shipmentId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetShipmentQuery(shipmentId));

                return result.ToHttpResult();
            });

        group.MapGet("/order/{orderId:guid}",
            [Authorize] async (
                Guid orderId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetShipmentByOrderQuery(orderId));

                return result.ToHttpResult();
            });

        group.MapGet("/my",
            [Authorize] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetMyShipmentsQuery());

                return result.ToHttpResult();
            });

        return app;
    }
}