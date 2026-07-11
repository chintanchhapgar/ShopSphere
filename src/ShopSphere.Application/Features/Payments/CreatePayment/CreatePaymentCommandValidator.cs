using FluentValidation;

namespace ShopSphere.Application.Features.Payments.CreatePayment;

public sealed class CreatePaymentCommandValidator
    : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Method)
            .IsInEnum();
    }
}