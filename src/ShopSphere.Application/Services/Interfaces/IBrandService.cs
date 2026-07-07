using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Services.Interfaces;

public interface IBrandService
{
    Task<Result<Brand>> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Result> EnsureActiveAsync(
        Guid id,
        CancellationToken cancellationToken);
}