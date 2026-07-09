using MediatR;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.Admin.GetOrders;

public sealed class GetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, Result<IReadOnlyList<AdminOrderListDto>>>
{
    private readonly IOrderQueries _orderQueries;

    public GetOrdersQueryHandler(
        IOrderQueries orderQueries)
    {
        _orderQueries = orderQueries;
    }

    public async Task<Result<IReadOnlyList<AdminOrderListDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderQueries.GetAllAsync(
            cancellationToken);

        return Result<IReadOnlyList<AdminOrderListDto>>
            .Success(orders);
    }
}