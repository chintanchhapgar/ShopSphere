using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Addresses.SetDefaultAddress;

public sealed record SetDefaultAddressCommand(Guid AddressId)
    : IRequest<Result>;