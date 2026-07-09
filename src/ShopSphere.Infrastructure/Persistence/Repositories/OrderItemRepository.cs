using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

internal sealed class OrderItemRepository : IOrderItemRepository
{
    private readonly ApplicationDbContext _context;

    public OrderItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        IEnumerable<OrderItem> orderItems,
        CancellationToken cancellationToken = default)
    {
        await _context.OrderItems.AddRangeAsync(
            orderItems,
            cancellationToken);
    }
}