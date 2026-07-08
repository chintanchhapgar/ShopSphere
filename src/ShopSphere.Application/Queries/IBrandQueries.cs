using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Features.Categories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Application.Queries
{
    public interface IBrandQueries
    {
        Task<IReadOnlyList<BrandDto>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<BrandDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
