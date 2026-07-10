namespace ShopSphere.Application.Features.Addresses.GetAddresses;

public sealed record AddressResponse(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault);