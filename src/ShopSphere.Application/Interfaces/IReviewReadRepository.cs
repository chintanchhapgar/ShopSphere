using ShopSphere.Application.Features.Reviews.GetProductReviews;

namespace ShopSphere.Application.Interfaces;

public interface IReviewReadRepository
{
    Task<IReadOnlyList<ProductReviewDto>> GetApprovedByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);
}