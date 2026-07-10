using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Addresses.GetAddresses;

public sealed record GetAddressesQuery()
    : IRequest<Result<IReadOnlyList<AddressResponse>>>;