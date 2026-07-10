using MediatR;
using ShopSphere.Application.Common.Models;
using ShopSphere.Application.Features.Products.SearchProducts;
using ShopSphere.Contracts.Common;

public sealed record SearchProductsQuery(
    ProductSearchRequest Request)
    : IRequest<Result<PagedResult<ProductSearchResponse>>>;