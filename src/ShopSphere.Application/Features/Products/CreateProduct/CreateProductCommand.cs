using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    string SKU,
    decimal BasePrice,
    decimal? CostPrice,
    Guid CategoryId,
    Guid BrandId,
    string Slug,
    string? Barcode,
    decimal? Weight)
    : IRequest<Result<Guid>>;