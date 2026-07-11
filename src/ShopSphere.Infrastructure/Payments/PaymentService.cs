using ShopSphere.Application.Features.Payments.PaymentGateway;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Payments;

public sealed class PaymentService
    : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IPaymentGateway _paymentGateway;

    public PaymentService(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IShipmentRepository shipmentRepository,
    IBackgroundJobService backgroundJobs,
    IPaymentGateway paymentGateway)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
        _backgroundJobs = backgroundJobs;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result<Guid>> CreatePaymentAsync(
        Guid orderId,
        PaymentMethod method,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
        {
            return Result<Guid>.Failure(
                PaymentErrors.OrderNotFound);
        }

        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        if (existingPayment is not null)
        {
            return Result<Guid>.Failure(
                PaymentErrors.AlreadyExists);
        }

        var payment = Payment.Create(
            order.Id,
            order.TotalAmount,
            method);

        await _paymentRepository.AddAsync(
            payment,
            cancellationToken);

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        var gatewayResponse =
            await _paymentGateway.CreatePaymentAsync(
                new PaymentGatewayRequest
                {
                    PaymentId = payment.Id,
                    Amount = payment.Amount,
                    Method = payment.Method,
                    OrderNumber = order.OrderNumber
                },
                cancellationToken);

        payment.UpdateStatus(
            PaymentStatus.Pending,
            gatewayReference: gatewayResponse.GatewayReference);

        return Result<Guid>.Success(
            payment.Id,
            "Payment created successfully.");
    }

    public async Task<Result> MarkPaymentSucceededAsync(
        Guid paymentId,
        string transactionId,
        string gatewayReference,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            paymentId,
            cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                PaymentErrors.NotFound);
        }

        if (payment.Status == PaymentStatus.Paid)
        {
            return Result.Success();
        }

        var verified = await _paymentGateway.VerifyPaymentAsync(
            transactionId,
            cancellationToken);

                if (!verified)
                {
                    return Result.Failure(
                        PaymentErrors.InvalidTransaction);
                }

        payment.MarkPaid(
            transactionId,
            gatewayReference);

        var order = await _orderRepository.GetByIdAsync(
            payment.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result.Failure(
                PaymentErrors.OrderNotFound);
        }

        order.Confirm();

        var shipment =
            await _shipmentRepository.GetByOrderIdAsync(
                order.Id,
                cancellationToken);

        if (shipment is null)
        {
            shipment = Shipment.Create(order.Id);

            await _shipmentRepository.AddAsync(
                shipment,
                cancellationToken);
        }

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        _backgroundJobs.Enqueue<IEmailJob>(
            x => x.SendOrderConfirmationAsync(
                order.Id));

        _backgroundJobs.Enqueue<IEmailJob>(
            x => x.SendShipmentCreatedAsync(
                shipment.Id));

        return Result.Success(
            "Payment completed successfully.");
    }

    public async Task<Result> MarkPaymentFailedAsync(
    Guid paymentId,
    string reason,
    CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            paymentId,
            cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                PaymentErrors.NotFound);
        }

        payment.MarkFailed();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Payment marked as failed.");
    }

    public async Task<Result> RefundAsync(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            paymentId,
            cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                PaymentErrors.NotFound);
        }

        await _paymentGateway.RefundAsync(
            payment.TransactionId!,
            cancellationToken);

        payment.UpdateStatus(
            PaymentStatus.Refunded);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Payment refunded successfully.");
    }
}