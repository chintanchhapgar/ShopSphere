using MediatR;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Shipments.GetShipment;

public sealed class GetShipmentQueryHandler
    : IRequestHandler<GetShipmentQuery, Result<ShipmentDto>>
{
    private readonly IShipmentQueries _shipmentQueries;

    public GetShipmentQueryHandler(
        IShipmentQueries shipmentQueries)
    {
        _shipmentQueries = shipmentQueries;
    }

    public async Task<Result<ShipmentDto>> Handle(
        GetShipmentQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentQueries.GetByIdAsync(
            request.ShipmentId,
            cancellationToken);

        if (shipment is null)
        {
            return Result<ShipmentDto>.Failure(
                ShipmentErrors.NotFound);
        }

        return Result<ShipmentDto>.Success(shipment);
    }
}