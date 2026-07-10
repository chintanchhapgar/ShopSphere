using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class WishlistRepository
    : Repository<Wishlist>,
      IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(
        ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Wishlist?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<Wishlist?> GetByCustomerWithItemsAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.Wishlists
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId,
                cancellationToken);
    }
}