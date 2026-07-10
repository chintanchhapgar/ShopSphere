namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed record ProductReviewsResponse(
    ReviewStatisticsResponse Statistics,
    IReadOnlyList<ReviewResponse> Reviews);