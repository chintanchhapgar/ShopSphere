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
    private readonly IPaymentService _paymentService;
    public CreatePaymentCommandHandler(
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<Guid>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        return await _paymentService.CreatePaymentAsync(
            request.OrderId,
            request.Method,
            cancellationToken);
    }
}