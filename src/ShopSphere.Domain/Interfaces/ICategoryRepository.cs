using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken);

    Task<bool> HasChildrenAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> AddOrRestoreAsync(
        Category category,
        CancellationToken cancellationToken);
}