using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}