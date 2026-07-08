
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Queries
{
    public interface IProductQueries
    {
        Task<ProductDetailsDto?> GetDetailsAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<ProductDto>> GetAllAsync(
           CancellationToken cancellationToken);

        Task<ProductDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
