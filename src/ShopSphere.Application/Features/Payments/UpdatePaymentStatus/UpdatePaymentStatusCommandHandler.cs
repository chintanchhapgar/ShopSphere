using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Payments.UpdatePaymentStatus;

public sealed class UpdatePaymentStatusCommandHandler
    : IRequestHandler<UpdatePaymentStatusCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public UpdatePaymentStatusCommandHandler(
        IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> Handle(
        UpdatePaymentStatusCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            request.PaymentId,
            cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                PaymentErrors.NotFound);
        }

        try
        {
            payment.UpdateStatus(
                request.Status,
                request.TransactionId);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(
                PaymentErrors.InvalidStatus);
        }

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Payment status updated successfully.");
    }
}