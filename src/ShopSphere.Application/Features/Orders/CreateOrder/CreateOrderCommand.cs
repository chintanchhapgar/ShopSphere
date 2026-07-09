using MediatR;
using ShopSphere.Application.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    string ShippingName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country)
    : IRequest<Result<CreateOrderResponse>>;