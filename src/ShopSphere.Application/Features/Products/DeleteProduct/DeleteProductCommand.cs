using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id)
    : IRequest<Result>;