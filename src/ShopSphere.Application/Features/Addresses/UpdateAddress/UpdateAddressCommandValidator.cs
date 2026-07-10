using FluentValidation;

namespace ShopSphere.Application.Features.Addresses.UpdateAddress;

public sealed class UpdateAddressCommandValidator
    : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}