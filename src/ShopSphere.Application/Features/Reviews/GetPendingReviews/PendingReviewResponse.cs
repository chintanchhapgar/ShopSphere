namespace ShopSphere.Application.Features.Reviews.GetPendingReviews;

public sealed record PendingReviewResponse(
    Guid Id,
    Guid ProductId,
    Guid CustomerId,
    int Rating,
    string Comment,
    DateTime CreatedAtUtc);