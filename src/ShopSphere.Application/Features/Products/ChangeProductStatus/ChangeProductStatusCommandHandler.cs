using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.ChangeProductStatus;

public sealed class ChangeProductStatusCommandHandler
    : IRequestHandler<ChangeProductStatusCommand, Result>
{
    private readonly IProductRepository _repository;

    public ChangeProductStatusCommandHandler(
        IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        ChangeProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                ProductErrors.NotFound);
        }

        if (request.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            request.IsActive
                ? "Product activated successfully."
                : "Product deactivated successfully.");
    }
}