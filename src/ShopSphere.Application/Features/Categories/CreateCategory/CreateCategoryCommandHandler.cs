using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryCommandHandler(
        ICategoryRepository repository)
    {
        _repository = repository;
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

        await _repository.AddAsync(
            category,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            category.Id,
            "Category created successfully.");
    }
}