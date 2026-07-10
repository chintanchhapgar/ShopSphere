using ShopSphere.Application.Common.Models;
using ShopSphere.Application.Features.Products.SearchProducts;

namespace ShopSphere.Application.Interfaces;

public interface IProductReadRepository
{
    Task<PagedResult<ProductSearchResponse>> SearchAsync(
        ProductSearchRequest request,
        CancellationToken cancellationToken);
}