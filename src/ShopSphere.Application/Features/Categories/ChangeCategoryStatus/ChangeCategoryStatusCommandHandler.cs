using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.ChangeCategoryStatus;

public sealed class ChangeCategoryStatusCommandHandler
    : IRequestHandler<ChangeCategoryStatusCommand, Result>
{
    private readonly ICategoryRepository _repository;
    private readonly ICacheService _cacheService;

    public ChangeCategoryStatusCommandHandler(
        ICategoryRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        ChangeCategoryStatusCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        if (request.IsActive)
        {
            category.Activate();
        }
        else
        {
            category.Deactivate();
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "categories",
          cancellationToken);

        return Result.Success(
            request.IsActive
                ? "Category activated successfully."
                : "Category deactivated successfully.");
    }
}