using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed class GetProductReviewsQueryHandler
    : IRequestHandler<
        GetProductReviewsQuery,
        Result<IReadOnlyList<ReviewResponse>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetProductReviewsQueryHandler(
        IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<IReadOnlyList<ReviewResponse>>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews =
            await _reviewRepository.GetApprovedByProductIdAsync(
                request.ProductId,
                cancellationToken);

        var response = reviews
            .Select(x => new ReviewResponse(
                x.Id,
                x.CreatedBy ?? "Anonymous",
                x.Rating,
                x.Comment,
                x.CreatedAtUtc))
            .ToList();

        return Result<IReadOnlyList<ReviewResponse>>
            .Success(response);
    }
}