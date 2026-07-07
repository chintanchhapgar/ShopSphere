using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Categories.ChangeCategoryStatus;

public sealed record ChangeCategoryStatusCommand(
    Guid Id,
    bool IsActive)
    : IRequest<Result>;