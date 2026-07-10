using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Addresses.GetAddresses;

public sealed class GetAddressesQueryHandler
    : IRequestHandler<
        GetAddressesQuery,
        Result<IReadOnlyList<AddressResponse>>>
{
    private readonly IAddressReadRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetAddressesQueryHandler(
        IAddressReadRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<AddressResponse>>> Handle(
        GetAddressesQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result<IReadOnlyList<AddressResponse>>
                .Failure(UserErrors.Unauthorized);
        }

        var addresses = await _repository.GetByCustomerAsync(
            customerId,
            cancellationToken);

        return Result<IReadOnlyList<AddressResponse>>
            .Success(addresses);
    }
}