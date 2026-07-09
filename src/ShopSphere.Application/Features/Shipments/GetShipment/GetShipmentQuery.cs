using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Shipments.GetShipment;

public sealed record GetShipmentQuery(
    Guid ShipmentId)
    : IRequest<Result<ShipmentDto>>;