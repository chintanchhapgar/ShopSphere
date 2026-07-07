using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.ChangeProductStatus;

public sealed record ChangeProductStatusCommand(
    Guid Id,
    bool IsActive)
    : IRequest<Result>;