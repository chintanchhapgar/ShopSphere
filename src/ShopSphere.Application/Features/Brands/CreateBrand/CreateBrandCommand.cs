using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands.CreateBrand;

public sealed record CreateBrandCommand(
    string Name,
    string? Description)
    : IRequest<Result<Guid>>;