using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Shipments.CreateShipment;

public sealed record CreateShipmentCommand(
    Guid OrderId)
    : IRequest<Result<Guid>>;