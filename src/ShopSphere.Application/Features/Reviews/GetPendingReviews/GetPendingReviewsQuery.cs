using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Reviews.GetPendingReviews;

public sealed record GetPendingReviewsQuery
    : IRequest<Result<IReadOnlyList<PendingReviewResponse>>>;