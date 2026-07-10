using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Addresses.CreateAddress;

public sealed class CreateAddressCommandHandler
    : IRequestHandler<CreateAddressCommand, Result<Guid>>
{
    private const int MaxAddresses = 10;

    private readonly IAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateAddressCommandHandler(
        IAddressRepository addressRepository,
        ICurrentUserService currentUserService)
    {
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        CreateAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result<Guid>.Failure(
                UserErrors.Unauthorized);
        }

        var totalAddresses = await _addressRepository
            .CountByCustomerAsync(
                customerId,
                cancellationToken);

        if (totalAddresses >= MaxAddresses)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "ADDRESS_LIMIT",
                    "You can save a maximum of 10 addresses."));
        }

        var address = new Address(
            customerId,
            request.FullName,
            request.PhoneNumber,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country);

        // First address becomes default automatically
        if (totalAddresses == 0)
        {
            address.SetDefault(true);
        }
        else if (request.IsDefault)
        {
            var currentDefault =
                await _addressRepository.GetDefaultAsync(
                    customerId,
                    cancellationToken);

            currentDefault?.SetDefault(false);

            address.SetDefault(true);
        }

        await _addressRepository.AddAsync(
            address,
            cancellationToken);

        await _addressRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(address.Id);
    }
}