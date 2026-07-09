using MediatR;
using ShopSphere.Application.Features.Shipments.GetShipment;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Shipments.GetShipmentByOrder;

public sealed class GetShipmentByOrderQueryHandler
    : IRequestHandler<GetShipmentByOrderQuery, Result<ShipmentDto>>
{
    private readonly IShipmentQueries _shipmentQueries;

    public GetShipmentByOrderQueryHandler(
        IShipmentQueries shipmentQueries)
    {
        _shipmentQueries = shipmentQueries;
    }

    public async Task<Result<ShipmentDto>> Handle(
        GetShipmentByOrderQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentQueries.GetByOrderIdAsync(
            request.OrderId,
            cancellationToken);

        if (shipment is null)
        {
            return Result<ShipmentDto>.Failure(
                ShipmentErrors.NotFound);
        }

        return Result<ShipmentDto>.Success(shipment);
    }
}