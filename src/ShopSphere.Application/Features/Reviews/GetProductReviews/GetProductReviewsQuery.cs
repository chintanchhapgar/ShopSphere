using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Reviews.GetProductReviews;

public sealed record GetProductReviewsQuery(
    Guid ProductId)
    : IRequest<Result<IReadOnlyList<ReviewResponse>>>;