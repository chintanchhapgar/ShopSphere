using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.GetProducts;

public sealed record GetProductsQuery
    : IRequest<Result<IReadOnlyList<ProductDto>>>;