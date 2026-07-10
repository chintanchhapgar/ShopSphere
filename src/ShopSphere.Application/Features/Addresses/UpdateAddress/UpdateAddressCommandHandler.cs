using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Addresses.UpdateAddress;

public sealed class UpdateAddressCommandHandler
    : IRequestHandler<UpdateAddressCommand, Result>
{
    private readonly IAddressRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAddressCommandHandler(
        IAddressRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        UpdateAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result.Failure(UserErrors.Unauthorized);
        }

        var address = await _repository.GetByIdForCustomerAsync(
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

        address.Update(
            request.FullName,
            request.PhoneNumber,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Address updated successfully.");
    }
}