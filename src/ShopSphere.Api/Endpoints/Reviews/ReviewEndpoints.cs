using MediatR;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Reviews.AddReview;
using ShopSphere.Application.Features.Reviews.GetProductReviews;

namespace ShopSphere.Api.Endpoints.Reviews;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews")
             .WithTags("Reviews")
            .RequireAuthorization();

        group.MapPost(
            "/{productId:guid}",
            async (
                Guid productId,
                AddReviewRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new AddReviewCommand(
                        productId,
                        request.Rating,
                        request.Comment),
                    cancellationToken);

                return result.ToHttpResult();
            });

        group.MapGet(
            "/{productId:guid}",
            async (
                Guid productId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetProductReviewsQuery(productId),
                    cancellationToken);

                return result.ToHttpResult();
            })
            .AllowAnonymous();

        return app;
    }
}

public sealed record AddReviewRequest(
    int Rating,
    string Comment);