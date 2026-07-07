using FluentValidation;

namespace ShopSphere.Application.Features.Brands.CreateBrand;

public sealed class CreateBrandCommandValidator
    : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}