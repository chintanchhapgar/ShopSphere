using MediatR;
using Microsoft.AspNetCore.Authorization;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Coupons.CreateCoupon;
using ShopSphere.Application.Features.Coupons.DeleteCoupon;
using ShopSphere.Application.Features.Coupons.GetCouponById;
using ShopSphere.Application.Features.Coupons.GetCoupons;
using ShopSphere.Application.Features.Coupons.UpdateCoupon;

namespace ShopSphere.Api.Endpoints.Coupons;

public static class CouponEndpoints
{
    public static IEndpointRouteBuilder MapCouponEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/coupons")
            .WithTags("Coupons");

        group.MapPost("/",
            [Authorize(Roles = "Admin")] async (
                CreateCouponCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.ToCreatedHttpResult(
                    $"/api/coupons/{result.Value}");
            });

        group.MapGet("/",
            [Authorize(Roles = "Admin")] async (
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetCouponsQuery());

                return result.ToHttpResult();
            });

        group.MapGet("/{id:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new GetCouponByIdQuery(id));

                return result.ToHttpResult();
            });

        group.MapPut("/{id:guid}",
            [Authorize(Roles = "Admin")] async (
                Guid id,
                UpdateCouponCommand command,
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
                    new DeleteCouponCommand(id));

                return result.ToHttpResult();
            });

        return app;
    }
}