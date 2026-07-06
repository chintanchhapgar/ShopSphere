using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}