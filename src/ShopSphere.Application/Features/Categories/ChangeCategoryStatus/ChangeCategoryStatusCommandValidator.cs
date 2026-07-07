using FluentValidation;

namespace ShopSphere.Application.Features.Categories.ChangeCategoryStatus;

public sealed class ChangeCategoryStatusCommandValidator
    : AbstractValidator<ChangeCategoryStatusCommand>
{
    public ChangeCategoryStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}