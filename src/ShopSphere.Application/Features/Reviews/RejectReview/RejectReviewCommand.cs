using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Reviews.RejectReview;

public sealed record RejectReviewCommand(
    Guid ReviewId)
    : IRequest<Result>;