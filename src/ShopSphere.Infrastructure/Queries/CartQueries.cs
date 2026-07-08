using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Carts.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class CartQueries : ICartQueries
{
    private readonly ApplicationDbContext _context;

    public CartQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var items = await _context.CartItems
            .AsNoTracking()
            .Where(x => x.Cart.CustomerId == customerId)
            .OrderBy(x => x.Product.Name)
            .Select(x => new CartItemDto(
                x.Id,
                x.ProductId,
                x.Product.Name,
                x.Product.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                x.Quantity,
                x.UnitPrice,
                x.Quantity * x.UnitPrice))
            .ToListAsync(cancellationToken);

        return new CartDto(
            items,
            items.Sum(x => x.Subtotal));
    }
}