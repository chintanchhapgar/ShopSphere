using FluentValidation;

namespace ShopSphere.Application.Features.Orders.CreateOrder;

public sealed class CreateOrderCommandValidator
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty();
    }
}