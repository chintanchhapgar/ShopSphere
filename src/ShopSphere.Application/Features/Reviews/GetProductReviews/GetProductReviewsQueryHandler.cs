using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed class GetProductReviewsQueryHandler
    : IRequestHandler<
        GetProductReviewsQuery,
        Result<ProductReviewsResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewReadRepository _reviewReadRepository;
    public GetProductReviewsQueryHandler(
        IReviewRepository reviewRepository,
        IReviewReadRepository reviewReadRepository)
    {
        _reviewRepository = reviewRepository;
        _reviewReadRepository = reviewReadRepository;
    }

    public async Task<Result<ProductReviewsResponse>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewReadRepository.GetApprovedByProductIdAsync(
        request.ProductId,
        cancellationToken);

        var reviewResponses = reviews
            .Select(x => new ReviewResponse(
                x.Id,
                x.CustomerName ?? "Anonymous",
                x.Rating,
                x.Comment,
                x.CreatedAtUtc))
            .ToList();

        var statistics = new ReviewStatisticsResponse(
            reviews.Any() ? Math.Round(reviews.Average(x => x.Rating), 1) : 0,
            reviews.Count,
            reviews.Count(x => x.Rating == 5),
            reviews.Count(x => x.Rating == 4),
            reviews.Count(x => x.Rating == 3),
            reviews.Count(x => x.Rating == 2),
            reviews.Count(x => x.Rating == 1));

        var response = new ProductReviewsResponse(
            statistics,
            reviewResponses);

        return Result<ProductReviewsResponse>.Success(
            response);
    }
}