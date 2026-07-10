using MediatR;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Reviews.ApproveReview;
using ShopSphere.Application.Features.Reviews.GetPendingReviews;
using ShopSphere.Application.Features.Reviews.RejectReview;

namespace ShopSphere.Api.Endpoints.Reviews;

public static class AdminReviewEndpoints
{
    public static IEndpointRouteBuilder MapAdminReviewEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/reviews")
            .WithTags("Admin Reviews")
            .RequireAuthorization(policy =>
                policy.RequireRole("Admin"));

        group.MapGet(
            "/pending",
            async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetPendingReviewsQuery(),
                    cancellationToken);

                return result.ToHttpResult();
            });

        group.MapPut(
            "/{id:guid}/approve",
            async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new ApproveReviewCommand(id),
                    cancellationToken);

                return result.ToHttpResult();
            });

        group.MapPut(
            "/{id:guid}/reject",
            async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new RejectReviewCommand(id),
                    cancellationToken);

                return result.ToHttpResult();
            });

        return app;
    }
}