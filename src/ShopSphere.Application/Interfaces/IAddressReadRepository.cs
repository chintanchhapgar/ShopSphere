using ShopSphere.Application.Features.Addresses.GetAddresses;

namespace ShopSphere.Application.Interfaces;

public interface IAddressReadRepository
{
    Task<IReadOnlyList<AddressResponse>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken);
}