using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.ProductImages.SetPrimaryProductImage;

public sealed record SetPrimaryProductImageCommand(
    Guid ProductId,
    Guid ImageId)
    : IRequest<Result>;