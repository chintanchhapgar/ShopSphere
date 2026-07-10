using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.RejectReview;

public sealed class RejectReviewCommandHandler
    : IRequestHandler<RejectReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;

    public RejectReviewCommandHandler(
        IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result> Handle(
        RejectReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(
            request.ReviewId,
            cancellationToken);

        if (review is null)
        {
            return Result.Failure(
                ReviewErrors.NotFound);
        }

        review.Reject();

        _reviewRepository.Update(review);

        await _reviewRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Review rejected successfully.");
    }
}