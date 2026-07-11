using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Payments.CreatePayment;
using ShopSphere.Application.Features.Payments.GetPayment;
using ShopSphere.Application.Features.Payments.PaymentFailed;
using ShopSphere.Application.Features.Payments.PaymentSucceeded;
using ShopSphere.Application.Features.Payments.RefundPayment;
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


        group.MapGet("/{orderId:guid}/payment",
            [Authorize] async (
                Guid orderId,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetPaymentQuery(orderId));

                return result.ToHttpResult();
            });

        group.MapPost("/{paymentId:guid}/success",
            [Authorize(Roles = "Admin")] async (
                Guid paymentId,
                PaymentSucceededRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new PaymentSucceededCommand(
                        paymentId,
                        request.TransactionId));

                return result.ToHttpResult();
            });

        group.MapPost("/{paymentId:guid}/failed",
            [Authorize(Roles = "Admin")] async (
                Guid paymentId,
                PaymentFailedRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(
            new PaymentFailedCommand(
                paymentId,
                request.Reason));

                return result.ToHttpResult();
            });

        group.MapPost("/{paymentId:guid}/refund",
            [Authorize(Roles = "Admin")] async (
                Guid paymentId,
                ISender sender) =>
            {
                var result = await sender.Send(
            new RefundPaymentCommand(paymentId));

                return result.ToHttpResult();
            });

        return app;
    }
}