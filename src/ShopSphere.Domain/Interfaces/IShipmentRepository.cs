using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
    Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken);

}