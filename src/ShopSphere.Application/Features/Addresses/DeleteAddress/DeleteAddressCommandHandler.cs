using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Addresses.DeleteAddress;

public sealed class DeleteAddressCommandHandler
    : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IAddressRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAddressCommandHandler(
        IAddressRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        DeleteAddressCommand request,
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

        var addresses = await _repository.GetByCustomerAsync(
            customerId,
            cancellationToken);

        if (address.IsDefault && addresses.Count > 1)
        {
            var nextDefault = addresses
                .First(x => x.Id != address.Id);

            nextDefault.SetDefault(true);
        }

        address.Delete();
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(
            "Address deleted successfully.");
    }
}