using FluentValidation;

namespace ShopSphere.Application.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryValidator
    : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x)
            .Must(x => x.Id != x.ParentCategoryId)
            .WithMessage("Category cannot be its own parent.");
    }
}