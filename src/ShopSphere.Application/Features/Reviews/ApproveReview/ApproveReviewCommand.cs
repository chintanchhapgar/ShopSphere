using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Reviews.ApproveReview;

public sealed record ApproveReviewCommand(
    Guid ReviewId)
    : IRequest<Result>;