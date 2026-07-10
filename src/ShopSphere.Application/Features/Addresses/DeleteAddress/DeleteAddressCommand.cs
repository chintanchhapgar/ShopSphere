using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Addresses.DeleteAddress;

public sealed record DeleteAddressCommand(Guid AddressId)
    : IRequest<Result>;