using Microsoft.Extensions.Logging;
using ShopSphere.Application.Features.Payments.PaymentGateway;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Services;

namespace ShopSphere.Infrastructure.Payments;

public sealed class PaymentService
    : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditService _auditService;
    private readonly ILogger<PaymentService> _logger;
    public PaymentService(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IShipmentRepository shipmentRepository,
    IBackgroundJobService backgroundJobs,
    IPaymentGateway paymentGateway,
    IAuditService auditService,
    ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
        _backgroundJobs = backgroundJobs;
        _paymentGateway = paymentGateway;
        _auditService = auditService;
        _logger = logger;
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

        _logger.LogInformation(
            "Payment {PaymentId} created for Order {OrderNumber}. Amount: {Amount}. Method: {Method}",
            payment.Id,
            order.OrderNumber,
            payment.Amount,
            payment.Method);

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

        _logger.LogInformation(
            "Payment success email queued for Order {OrderNumber}.",
            order.OrderNumber);

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
            _logger.LogWarning(
                "Payment verification failed. TransactionId: {TransactionId}",
                transactionId);

            return Result.Failure(
                 PaymentErrors.InvalidTransaction);
        }

        payment.MarkPaid(
            transactionId,
            gatewayReference);

        _logger.LogInformation(
            "Payment {PaymentId} completed. TransactionId: {TransactionId}",
            payment.Id,
            transactionId);

        await _auditService.LogAsync(
            AuditActions.PaymentSucceeded,
            AuditEntities.Payment,
            payment.Id,
            $"Payment successful. Transaction: {transactionId}.",
            cancellationToken);

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

        await _auditService.LogAsync(
            AuditActions.PaymentFailed,
            AuditEntities.Payment,
            payment.Id,
            $"Payment failed for order '{payment.Order.OrderNumber}'.",
            cancellationToken);

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

        await _auditService.LogAsync(
            AuditActions.PaymentRefunded,
            AuditEntities.Payment,
            payment.Id,
            $"Refund processed. Transaction: {payment.TransactionId}.",
            cancellationToken);

        return Result.Success(
            "Payment refunded successfully.");
    }
}