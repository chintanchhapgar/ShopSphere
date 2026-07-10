using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.GetPendingReviews;

public sealed class GetPendingReviewsQueryHandler
    : IRequestHandler<
        GetPendingReviewsQuery,
        Result<IReadOnlyList<PendingReviewResponse>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetPendingReviewsQueryHandler(
        IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<IReadOnlyList<PendingReviewResponse>>> Handle(
        GetPendingReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetPendingAsync(
            cancellationToken);

        var response = reviews
            .Select(x => new PendingReviewResponse(
                x.Id,
                x.ProductId,
                x.CustomerId,
                x.Rating,
                x.Comment,
                x.CreatedAtUtc))
            .ToList();

        return Result<IReadOnlyList<PendingReviewResponse>>
            .Success(response);
    }
}