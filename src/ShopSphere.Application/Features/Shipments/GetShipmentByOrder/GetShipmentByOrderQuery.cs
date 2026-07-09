using MediatR;
using ShopSphere.Application.Features.Shipments.GetShipment;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Shipments.GetShipmentByOrder;

public sealed record GetShipmentByOrderQuery(
    Guid OrderId)
    : IRequest<Result<ShipmentDto>>;