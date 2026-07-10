using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Addresses.UpdateAddress;

public sealed record UpdateAddressCommand(
    Guid AddressId,
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country)
    : IRequest<Result>;