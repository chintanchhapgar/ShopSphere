using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
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
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    public CreatePaymentCommandHandler(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IUserService userService,
    INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _userService = userService;
        _notificationService = notificationService;
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

        var user = await _userService.GetByIdAsync(
            order.UserId.ToString(),
            cancellationToken);

            if (user is not null)
            {
                await _notificationService.SendPaymentSucceededAsync(
                    new PaymentSucceededEmailModel(
                        user.FullName,
                        user.Email,
                        order.OrderNumber,
                        payment.Amount,
                        payment.Method.ToString(),
                        payment.TransactionId),
                    cancellationToken);
            }

        return Result<Guid>.Success(
            payment.Id,
            "Payment created successfully.");
    }
}