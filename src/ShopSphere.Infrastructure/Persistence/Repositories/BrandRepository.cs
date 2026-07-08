using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class BrandRepository
    : Repository<Brand>, IBrandRepository
{
    public BrandRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return await Entities.AnyAsync(
            x => x.Name == name &&
                 x.Id != excludeId,
            cancellationToken);
    }
}