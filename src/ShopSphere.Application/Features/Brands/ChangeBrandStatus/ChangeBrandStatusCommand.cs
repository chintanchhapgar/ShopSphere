using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands.ChangeBrandStatus;

public sealed record ChangeBrandStatusCommand(
    Guid Id,
    bool IsActive)
    : IRequest<Result>;