using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Orders.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailsDto>>
{
    private readonly IOrderQueries _orderQueries;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderByIdQueryHandler(
        IOrderQueries orderQueries,
        ICurrentUserService currentUserService)
    {
        _orderQueries = orderQueries;
        _currentUserService = currentUserService;
    }

    public async Task<Result<OrderDetailsDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var customerId))
        {
            return Result<OrderDetailsDto>.Failure(
                UserErrors.Unauthorized);
        }

        var order = await _orderQueries.GetByIdAsync(
            customerId,
            request.Id,
            cancellationToken);

        if (order is null)
        {
            return Result<OrderDetailsDto>.Failure(
                OrderErrors.NotFound);
        }

        return Result<OrderDetailsDto>.Success(order);
    }
}