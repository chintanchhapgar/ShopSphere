using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                cancellationToken);
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        await _context.Payments.AddAsync(
            payment,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(
                x => x.TransactionId == transactionId,
                cancellationToken);
    }

    public async Task<Payment?> GetByGatewayReferenceAsync(
        string gatewayReference,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(
                x => x.GatewayReference == gatewayReference,
                cancellationToken);
    }
}