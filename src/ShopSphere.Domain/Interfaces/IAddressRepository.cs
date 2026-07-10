using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IAddressRepository : IRepository<Address>
{
    Task<IReadOnlyList<Address>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<Address?> GetByIdForCustomerAsync(
        Guid addressId,
        Guid customerId,
        CancellationToken cancellationToken);

    Task<Address?> GetDefaultAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid customerId,
        Guid addressId,
        CancellationToken cancellationToken);

    Task<int> CountByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    void Update(Address address);
}