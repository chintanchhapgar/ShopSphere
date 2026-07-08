using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.ProductImages.UpdateProductImageDisplayOrder;

public sealed record UpdateProductImageDisplayOrderCommand(
    Guid ProductId,
    Guid ImageId,
    int DisplayOrder)
    : IRequest<Result>;