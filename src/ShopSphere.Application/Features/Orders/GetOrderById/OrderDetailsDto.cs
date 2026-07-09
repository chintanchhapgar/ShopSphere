namespace ShopSphere.Application.Features.Orders.GetOrderById;

public sealed record OrderDetailsDto(
    Guid Id,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    decimal SubTotal,
    decimal TaxAmount,
    decimal ShippingAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    string ShippingName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    IReadOnlyList<OrderItemDto> Items);