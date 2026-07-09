using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Shipments.GetShipment;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class ShipmentQueries
    : IShipmentQueries
{
    private readonly ApplicationDbContext _context;

    public ShipmentQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentDto?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .AsNoTracking()
            .Where(x => x.Id == shipmentId)
            .Select(x => new ShipmentDto(
                x.Id,
                x.OrderId,
                x.Status,
                x.TrackingNumber,
                x.Carrier,
                x.ShippedAtUtc,
                x.DeliveredAtUtc,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    public async Task<ShipmentDto?> GetByOrderIdAsync(
    Guid orderId,
    CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .Select(x => new ShipmentDto(
                x.Id,
                x.OrderId,
                x.Status,
                x.TrackingNumber,
                x.Carrier,
                x.ShippedAtUtc,
                x.DeliveredAtUtc,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    public async Task<IReadOnlyList<ShipmentDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .AsNoTracking()
            .Where(x => x.Order.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ShipmentDto(
                x.Id,
                x.OrderId,
                x.Status,
                x.TrackingNumber,
                x.Carrier,
                x.ShippedAtUtc,
                x.DeliveredAtUtc,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}