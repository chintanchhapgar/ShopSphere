using FluentValidation;

namespace ShopSphere.Application.Features.Brands.UpdateBrand;

public sealed class UpdateBrandCommandValidator
    : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}