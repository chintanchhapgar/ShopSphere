using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Reviews.ApproveReview;

public sealed class ApproveReviewCommandHandler
    : IRequestHandler<ApproveReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;

    public ApproveReviewCommandHandler(
        IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result> Handle(
        ApproveReviewCommand request,
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

        review.Approve();

        _reviewRepository.Update(review);

        await _reviewRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Review approved successfully.");
    }
}