using ShopSphere.Application.Features.Shipments.GetShipment;

namespace ShopSphere.Application.Queries;

public interface IShipmentQueries
{
    Task<ShipmentDto?> GetByIdAsync(
       Guid shipmentId,
       CancellationToken cancellationToken);

    Task<ShipmentDto?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ShipmentDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}