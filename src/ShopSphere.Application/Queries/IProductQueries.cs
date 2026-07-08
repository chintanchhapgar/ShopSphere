
using ShopSphere.Application.Features.Products.Common;

namespace ShopSphere.Application.Queries
{
    public interface IProductQueries
    {
        Task<IReadOnlyList<ProductDto>> GetAllAsync(
           CancellationToken cancellationToken);

        Task<ProductDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
