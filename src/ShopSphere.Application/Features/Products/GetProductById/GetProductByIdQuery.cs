using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<Result<ProductDto>>;