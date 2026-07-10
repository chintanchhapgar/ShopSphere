using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Addresses.SetDefaultAddress;

public sealed class SetDefaultAddressCommandHandler
    : IRequestHandler<SetDefaultAddressCommand, Result>
{
    private readonly IAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUserService;

    public SetDefaultAddressCommandHandler(
        IAddressRepository addressRepository,
        ICurrentUserService currentUserService)
    {
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        SetDefaultAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result.Failure(UserErrors.Unauthorized);
        }

        var address = await _addressRepository.GetByIdForCustomerAsync(
            request.AddressId,
            customerId,
            cancellationToken);

        if (address is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "ADDRESS_NOT_FOUND",
                    "Address not found."));
        }

        var currentDefault =
            await _addressRepository.GetDefaultAsync(
                customerId,
                cancellationToken);

        if (currentDefault is not null &&
            currentDefault.Id != address.Id)
        {
            currentDefault.SetDefault(false);
        }

        address.SetDefault(true);

        await _addressRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Default address updated.");
    }
}