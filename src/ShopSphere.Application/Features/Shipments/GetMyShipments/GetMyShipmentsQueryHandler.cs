using MediatR;
using ShopSphere.Application.Features.Shipments.GetShipment;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Shipments.GetMyShipments;

public sealed class GetMyShipmentsQueryHandler
    : IRequestHandler<GetMyShipmentsQuery, Result<IReadOnlyList<ShipmentDto>>>
{
    private readonly IShipmentQueries _shipmentQueries;
    private readonly ICurrentUserService _currentUserService;

    public GetMyShipmentsQueryHandler(
        IShipmentQueries shipmentQueries,
        ICurrentUserService currentUserService)
    {
        _shipmentQueries = shipmentQueries;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<ShipmentDto>>> Handle(
        GetMyShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var userId))
        {
            return Result<IReadOnlyList<ShipmentDto>>.Failure(
                UserErrors.Unauthorized);
        }

        var shipments = await _shipmentQueries.GetByUserIdAsync(
            userId,
            cancellationToken);

        return Result<IReadOnlyList<ShipmentDto>>.Success(
            shipments);
    }
}