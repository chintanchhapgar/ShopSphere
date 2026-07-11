using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IAddressRepository _addressRepository;
    public CreateOrderCommandHandler(
    ICartRepository cartRepository,
    IInventoryRepository inventoryRepository,
    IOrderRepository orderRepository,
    IAddressRepository addressRepository,
    ICurrentUserService currentUserService,
    IUserService userService,
    INotificationService notificationService,
    IBackgroundJobService backgroundJobs)
    {
        _cartRepository = cartRepository;
        _inventoryRepository = inventoryRepository;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
        _userService = userService;
        _notificationService = notificationService;
        _backgroundJobs = backgroundJobs;
    }

    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var customerId))
        {
            return Result<CreateOrderResponse>.Failure(
                UserErrors.Unauthorized);
        }

        var cart = await _cartRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (cart is null || !cart.Items.Any())
        {
            return Result<CreateOrderResponse>.Failure(
                CartErrors.Empty);
        }

        var productIds = cart.Items
            .Select(x => x.ProductId)
            .ToList();

        var inventories = await _inventoryRepository.GetByProductIdsAsync(
            productIds,
            cancellationToken);

        var inventoryLookup = inventories.ToDictionary(x => x.ProductId);

        foreach (var item in cart.Items)
        {
            if (!inventoryLookup.TryGetValue(item.ProductId, out var inventory))
            {
                return Result<CreateOrderResponse>.Failure(
                    InventoryErrors.NotFound);
            }

            if (inventory.AvailableQuantity < item.Quantity)
            {
                return Result<CreateOrderResponse>.Failure(
                    InventoryErrors.InsufficientStock);
            }
        }


        var address = await _addressRepository.GetByIdAsync(
        request.AddressId,
        cancellationToken);

            if (address is null)
            {
                return Result<CreateOrderResponse>.Failure(
                    AddressErrors.NotFound);
            }


        var orderNumber =
            $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..23];

        var order = Order.Create(
            customerId,
            orderNumber,
            address.FullName,
            address.PhoneNumber,
            address.AddressLine1,
            address.AddressLine2,
            address.City,
            address.State,
            address.PostalCode,
            address.Country);

        foreach (var cartItem in cart.Items)
        {
            var imageUrl = cartItem.Product.Images
                .FirstOrDefault()?.ImageUrl;

            var orderItem = OrderItem.Create(
                cartItem.ProductId,
                cartItem.Product.Name,
                cartItem.Product.SKU,
                imageUrl,
                cartItem.UnitPrice,
                cartItem.Quantity);

            order.AddItem(orderItem);
        }

        // Apply coupon if present
        if (cart.CouponId.HasValue && cart.Coupon is not null)
        {
            order.ApplyCoupon(
                cart.Coupon.Id,
                cart.Coupon.Code,
                cart.DiscountAmount);
        }

        // Tax & Shipping
        order.SetCharges(
            taxAmount: 0,
            shippingAmount: 0);

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        // Reduce inventory
        foreach (var cartItem in cart.Items)
        {
            inventoryLookup[cartItem.ProductId]
                .DecreaseStock(cartItem.Quantity);
        }

        // Increment coupon usage & clear coupon
        cart.CompleteCheckout();

        // Remove all cart items
        _cartRepository.RemoveItems(cart);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        //var user = await _userService.GetByIdAsync(
        //    _currentUserService.UserId,
        //    cancellationToken);

        //if (user is not null)
        //{
        //    _backgroundJobs.Enqueue<IEmailJob>(
        //        x => x.SendOrderConfirmationAsync(order.Id));
        //}

        _backgroundJobs.Enqueue<IEmailJob>(
          x => x.SendOrderConfirmationAsync(order.Id));

        return Result<CreateOrderResponse>.Success(
            new CreateOrderResponse(
                order.Id,
                order.OrderNumber),
            "Order placed successfully.");
    }
}