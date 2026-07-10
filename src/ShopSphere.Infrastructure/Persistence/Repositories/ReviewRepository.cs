using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository
    : Repository<Review>,
      IReviewRepository
{
    public ReviewRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Review>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await Context.Reviews
            .Where(x =>
                x.ProductId == productId &&
                x.Status == ReviewStatus.Approved)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCustomerAsync(
        Guid productId,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await Context.Reviews
            .AnyAsync(
                x =>
                    x.ProductId == productId &&
                    x.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await Context.Reviews
            .Where(x =>
                x.ProductId == productId &&
                x.Status == ReviewStatus.Approved)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetPendingAsync(
    CancellationToken cancellationToken)
    {
        return await Context.Reviews
            .Where(x => x.Status == ReviewStatus.Pending)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Update(
        Review review)
    {
        Context.Reviews.Update(review);
    }
}