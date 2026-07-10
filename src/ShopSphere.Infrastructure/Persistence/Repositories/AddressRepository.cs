using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class AddressRepository
    : Repository<Address>,
      IAddressRepository
{
    public AddressRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Address>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await Entities
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Address?> GetByIdForCustomerAsync(
        Guid addressId,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await Entities.FirstOrDefaultAsync(
            x => x.Id == addressId &&
                 x.CustomerId == customerId,
            cancellationToken);
    }

    public async Task<Address?> GetDefaultAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await Entities.FirstOrDefaultAsync(
            x => x.CustomerId == customerId &&
                 x.IsDefault,
            cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid customerId,
        Guid addressId,
        CancellationToken cancellationToken)
    {
        return await Entities.AnyAsync(
            x => x.CustomerId == customerId &&
                 x.Id == addressId,
            cancellationToken);
    }

    public void Update(
        Address address)
    {
        Context.Addresses.Update(address);
    }

    public async Task<int> CountByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await Entities.CountAsync(
            x => x.CustomerId == customerId,
            cancellationToken);
    }
}