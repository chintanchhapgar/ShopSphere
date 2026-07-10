namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed record ReviewResponse(
    Guid Id,
    string CustomerName,
    int Rating,
    string Comment,
    DateTime CreatedAtUtc);