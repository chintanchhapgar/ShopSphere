using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Payments.GetPayment;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class PaymentQueries : IPaymentQueries
{
    private readonly ApplicationDbContext _context;

    public PaymentQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .Select(x => new PaymentDto(
                x.Id,
                x.OrderId,
                x.Amount,
                x.Status,
                x.Method,
                x.TransactionId,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync(
                cancellationToken);
    }
}