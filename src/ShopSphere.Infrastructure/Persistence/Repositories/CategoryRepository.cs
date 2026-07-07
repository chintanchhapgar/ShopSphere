using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository
    : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await Entities
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
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

    public async Task<bool> HasChildrenAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await Entities.AnyAsync(
            x => x.ParentCategoryId == id,
            cancellationToken);
    }
}