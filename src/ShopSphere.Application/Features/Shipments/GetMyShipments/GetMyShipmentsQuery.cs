using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Application.Features.Shipments.GetShipment;

namespace ShopSphere.Application.Features.Shipments.GetMyShipments;

public sealed record GetMyShipmentsQuery()
    : IRequest<Result<IReadOnlyList<ShipmentDto>>>;