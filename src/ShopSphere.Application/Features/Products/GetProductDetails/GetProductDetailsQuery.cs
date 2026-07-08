using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Contracts.Common;

public sealed record GetProductDetailsQuery(
    Guid Id)
    : IRequest<Result<ProductDetailsDto>>;