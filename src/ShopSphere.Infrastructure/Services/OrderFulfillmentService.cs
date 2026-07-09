using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Infrastructure.Services
{
    public sealed class OrderFulfillmentService : IOrderFulfillmentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShipmentRepository _shipmentRepository;

        public OrderFulfillmentService(
            IOrderRepository orderRepository,
            IShipmentRepository shipmentRepository)
        {
            _orderRepository = orderRepository;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Result> ConfirmOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken)
            {
                var order = await _orderRepository.GetByIdAsync(
                    orderId,
                    cancellationToken);

                if (order is null)
                    return Result.Failure(OrderErrors.NotFound);

                order.Confirm();

                var shipment = Shipment.Create(order.Id);

                await _shipmentRepository.AddAsync(
                    shipment,
                    cancellationToken);

                await _orderRepository.SaveChangesAsync(
                    cancellationToken);

                return Result.Success(
                    "Order confirmed successfully.");
        }
    }
}
