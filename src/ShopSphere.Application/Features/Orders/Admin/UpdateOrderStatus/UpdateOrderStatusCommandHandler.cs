using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.Admin.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result.Failure(
                OrderErrors.NotFound);
        }

        try
        {
            order.UpdateStatus(request.Status);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(
                OrderErrors.InvalidStatus);
        }

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Order status updated successfully.");
    }
}