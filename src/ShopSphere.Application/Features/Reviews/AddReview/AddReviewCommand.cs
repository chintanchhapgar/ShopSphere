using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Reviews.AddReview;

public sealed record AddReviewCommand(
    Guid ProductId,
    int Rating,
    string Comment)
    : IRequest<Result<Guid>>;