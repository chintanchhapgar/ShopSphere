using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Reviews.GetProductReviews;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ReviewReadRepository
    : IReviewReadRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductReviewDto>> GetApprovedByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await
            (from review in _context.Reviews
             join user in _context.Users
                 on review.CustomerId.ToString() equals user.Id
             where review.ProductId == productId
                && review.Status == ReviewStatus.Approved
             orderby review.CreatedAtUtc descending
             select new ProductReviewDto(
                 review.Id,
                 review.ProductId,
                 review.CustomerId,
                 $"{user.FirstName} {user.LastName}".Trim(),
                 review.Rating,
                 review.Comment,
                 review.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}