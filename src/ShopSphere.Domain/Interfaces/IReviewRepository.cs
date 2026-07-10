using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCustomerAsync(
        Guid productId,
        Guid customerId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Review>> GetPendingAsync(
    CancellationToken cancellationToken);

    Task<Review?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    void Update(Review review);
}