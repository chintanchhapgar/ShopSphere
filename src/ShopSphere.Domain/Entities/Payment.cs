using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class Payment : AuditableEntity
{
    private Payment()
    {
    }

    private Payment(
        Guid orderId,
        decimal amount,
        PaymentMethod method)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public PaymentStatus Status { get; private set; }

    public PaymentMethod Method { get; private set; }

    public string? TransactionId { get; private set; }

    public static Payment Create(
        Guid orderId,
        decimal amount,
        PaymentMethod method)
    {
        return new Payment(
            orderId,
            amount,
            method);
    }

    public void MarkPaid(
        string? transactionId = null)
    {
        Status = PaymentStatus.Paid;
        TransactionId = transactionId;
    }

    public void MarkFailed()
    {
        Status = PaymentStatus.Failed;
    }

    public void UpdateStatus(
    PaymentStatus status,
    string? transactionId = null)
    {
        if (Status == PaymentStatus.Paid &&
            status != PaymentStatus.Refunded)
        {
            throw new InvalidOperationException(
                "Paid payment cannot be changed.");
        }

        Status = status;

        if (!string.IsNullOrWhiteSpace(transactionId))
        {
            TransactionId = transactionId;
        }
    }
}