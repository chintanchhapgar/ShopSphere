using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Orders.GetMyOrders;

public sealed class GetMyOrdersQueryHandler
    : IRequestHandler<GetMyOrdersQuery, Result<IReadOnlyList<OrderListItemDto>>>
{
    private readonly IOrderQueries _orderQueries;
    private readonly ICurrentUserService _currentUserService;

    public GetMyOrdersQueryHandler(
        IOrderQueries orderQueries,
        ICurrentUserService currentUserService)
    {
        _orderQueries = orderQueries;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<OrderListItemDto>>> Handle(
        GetMyOrdersQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var customerId))
        {
            return Result<IReadOnlyList<OrderListItemDto>>.Failure(
                UserErrors.Unauthorized);
        }

        var orders = await _orderQueries.GetMyOrdersAsync(
            customerId,
            cancellationToken);

        return Result<IReadOnlyList<OrderListItemDto>>.Success(orders);
    }
}