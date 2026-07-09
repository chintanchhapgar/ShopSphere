using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Orders.Admin.GetOrders;
using ShopSphere.Application.Features.Orders.GetMyOrders;
using ShopSphere.Application.Features.Orders.GetOrderById;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class OrderQueries : IOrderQueries
{
    private readonly ApplicationDbContext _context;

    public OrderQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrderListItemDto>> GetMyOrdersAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(x => x.UserId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new OrderListItemDto(
                x.Id,
                x.OrderNumber,
                x.CreatedAtUtc,
                x.Status.ToString(),
                x.TotalAmount,
                x.Items.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(x => x.Id == orderId && x.UserId == customerId)
            .Select(x => new OrderDetailsDto(
                x.Id,
                x.OrderNumber,
                x.CreatedAtUtc,
                x.Status.ToString(),
                x.SubTotal,
                x.TaxAmount,
                x.ShippingAmount,
                x.DiscountAmount,
                x.TotalAmount,
                x.ShippingName,
                x.PhoneNumber,
                x.AddressLine1,
                x.AddressLine2,
                x.City,
                x.State,
                x.PostalCode,
                x.Country,
                x.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.SKU,
                    i.ImageUrl,
                    i.UnitPrice,
                    i.Quantity,
                    i.TotalPrice))
                .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminOrderListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new AdminOrderListDto(
                x.Id,
                x.OrderNumber,
                x.UserId,
                x.Status.ToString(),
                x.TotalAmount,
                x.Items.Count,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}