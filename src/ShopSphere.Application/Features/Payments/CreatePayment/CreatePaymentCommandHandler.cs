using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Payments.CreatePayment;

public sealed class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CreatePaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result<Guid>.Failure(
                PaymentErrors.OrderNotFound);
        }

        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

        if (existingPayment is not null)
        {
            return Result<Guid>.Failure(
                PaymentErrors.AlreadyExists);
        }

        var payment = Payment.Create(
            order.Id,
            order.TotalAmount,
            request.PaymentMethod);

        await _paymentRepository.AddAsync(
            payment,
            cancellationToken);

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            payment.Id,
            "Payment created successfully.");
    }
}