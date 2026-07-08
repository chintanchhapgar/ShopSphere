using FluentValidation;

namespace ShopSphere.Application.Features.ProductImages.UpdateProductImageDisplayOrder;

public sealed class UpdateProductImageDisplayOrderCommandValidator
    : AbstractValidator<UpdateProductImageDisplayOrderCommand>
{
    public UpdateProductImageDisplayOrderCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.ImageId)
            .NotEmpty();

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}