using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Services.Interfaces;

public interface IOrderFulfillmentService
{
    Task<Result> ConfirmOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}