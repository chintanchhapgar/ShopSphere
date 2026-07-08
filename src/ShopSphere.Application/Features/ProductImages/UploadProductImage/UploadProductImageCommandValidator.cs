using FluentValidation;

namespace ShopSphere.Application.Features.ProductImages.UploadProductImage;

public sealed class UploadProductImageCommandValidator
    : AbstractValidator<UploadProductImageCommand>
{
    public UploadProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.FileStream)
            .NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.FileSize)
            .GreaterThan(0);
    }
}