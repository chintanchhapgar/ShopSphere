using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _repository;
    private readonly ICacheService _cacheService;

    public CreateCategoryCommandHandler(
        ICategoryRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
         

        var category = new Category(
            request.Name,
            request.Description,
            request.ParentCategoryId);

        var exists = await _repository.ExistsByNameAsync(
                    request.Name,
                    null,
                    cancellationToken);
        if (exists)
        {
            return Result<Guid>.Failure(CategoryErrors.AlreadyExists);
        }

        var added = await _repository.AddOrRestoreAsync(
            category,
            cancellationToken);

        if (!added)
        {
            return Result<Guid>.Failure(
                CategoryErrors.AlreadyExists);
        }

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
           "categories",
           cancellationToken);

        return Result<Guid>.Success(
            category.Id,
            "Category created successfully.");
    }
}