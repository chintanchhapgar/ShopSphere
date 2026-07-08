using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.ProductImages.DeleteProductImage;

public sealed record DeleteProductImageCommand(
    Guid ProductId,
    Guid ImageId)
    : IRequest<Result>;