using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Addresses.CreateAddress;

public sealed record CreateAddressCommand(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault)
    : IRequest<Result<Guid>>;