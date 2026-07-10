namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed record ProductReviewDto(
    Guid Id,
    Guid ProductId,
    Guid CustomerId,
    string CustomerName,
    int Rating,
    string Comment,
    DateTime CreatedAtUtc);