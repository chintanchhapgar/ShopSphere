using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    decimal BasePrice,
    decimal? CostPrice,
    Guid CategoryId,
    Guid BrandId)
    : IRequest<Result>;