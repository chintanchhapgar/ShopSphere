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

    public async Task<bool> AddOrRestoreAsync(
    Category category,
    CancellationToken cancellationToken)
    {
        var existing = await Context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Name == category.Name,
                cancellationToken);

        if (existing is null)
        {
            await AddAsync(
                category,
                cancellationToken);

            return true;
        }

        if (!existing.IsDeleted)
        {
            return false;
        }

        existing.Restore();

        existing.Update(
            category.Name,
            category.Description,
            category.ParentCategoryId);

        return true;
    }
}