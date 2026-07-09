using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Persistence;
using ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class CartRepository
    : Repository<Cart>,
      ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<Cart?> GetByItemIdAsync(
        Guid itemId,
        CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Items.Any(i => i.Id == itemId),
                cancellationToken);
    }

    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
    }

    public void RemoveItems(Cart cart)
    {
        _context.CartItems.RemoveRange(cart.Items);
    }


    public async Task<Cart?> GetByCustomerWithItemsAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                cancellationToken);
    }
}