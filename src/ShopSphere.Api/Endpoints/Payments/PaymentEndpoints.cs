using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Payments.CreatePayment;
using ShopSphere.Application.Features.Payments.GetPayment;
using ShopSphere.Application.Features.Payments.UpdatePaymentStatus;
using ShopSphere.Contracts.Payments;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Api.Endpoints.Payments;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Payments");

        group.MapPost("/{id:guid}/payment",
            [Authorize] async (
                Guid id,
                PaymentRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new CreatePaymentCommand(
                        id,
                        request.PaymentMethod));

                return result.ToCreatedHttpResult(
                    $"/api/orders/{id}/payment");
            });

        group.MapPatch("/{paymentId:guid}/status",
            [Authorize(Roles = "Admin")] async (
                Guid paymentId,
                UpdatePaymentStatusRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new UpdatePaymentStatusCommand(
                        paymentId,
                        request.Status,
                        request.TransactionId));

                return result.ToHttpResult();
            });

        group.MapGet("/{orderId:guid}/payment",
            [Authorize] async (
                Guid orderId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetPaymentQuery(orderId));

                return result.ToHttpResult();
            });

        return app;
    }
}