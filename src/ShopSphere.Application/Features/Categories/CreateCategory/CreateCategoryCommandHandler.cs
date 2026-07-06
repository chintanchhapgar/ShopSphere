using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = new Category(
            request.Name,
            request.Description,
            request.ParentCategoryId);

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}