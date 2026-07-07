using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<Result<Category>> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Result> EnsureActiveAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Result> EnsureCanDeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}