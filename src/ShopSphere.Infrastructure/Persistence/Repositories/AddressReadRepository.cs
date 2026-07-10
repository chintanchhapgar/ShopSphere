using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Addresses.GetAddresses;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class AddressReadRepository
    : IAddressReadRepository
{
    private readonly ApplicationDbContext _context;

    public AddressReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AddressResponse>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new AddressResponse(
                x.Id,
                x.FullName,
                x.PhoneNumber,
                x.AddressLine1,
                x.AddressLine2,
                x.City,
                x.State,
                x.PostalCode,
                x.Country,
                x.IsDefault))
            .ToListAsync(cancellationToken);
    }
}