using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ShipmentRepository : IShipmentRepository
{
    private readonly ApplicationDbContext _context;

    public ShipmentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                cancellationToken);
    }

    public async Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken)
    {
        await _context.Shipments.AddAsync(
            shipment,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(
                x => x.Id == shipmentId,
                cancellationToken);
    }

    public async Task<Shipment?> GetByIdWithDetailsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(
                x => x.Id == shipmentId,
                cancellationToken);
    }
}