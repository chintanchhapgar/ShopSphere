using ShopSphere.Application.Features.Carts.Common;

namespace ShopSphere.Application.Queries;

public interface ICartQueries
{
    Task<CartDto?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);
}