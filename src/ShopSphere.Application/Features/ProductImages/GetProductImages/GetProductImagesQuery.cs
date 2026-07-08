using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.ProductImages.GetProductImages;

public sealed record GetProductImagesQuery(
    Guid ProductId)
    : IRequest<Result<IReadOnlyList<ProductImageDto>>>;