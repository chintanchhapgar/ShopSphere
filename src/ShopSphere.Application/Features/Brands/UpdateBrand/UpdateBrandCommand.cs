using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands.UpdateBrand;

public sealed record UpdateBrandCommand(
    Guid Id,
    string Name,
    string? Description)
    : IRequest<Result>;