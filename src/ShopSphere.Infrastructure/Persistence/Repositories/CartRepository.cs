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
}