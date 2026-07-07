using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IBrandRepository : IRepository<Brand>
{
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken);
}