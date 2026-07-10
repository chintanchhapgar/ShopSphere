namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed record ReviewStatisticsResponse(
    double AverageRating,
    int TotalReviews,
    int FiveStar,
    int FourStar,
    int ThreeStar,
    int TwoStar,
    int OneStar);