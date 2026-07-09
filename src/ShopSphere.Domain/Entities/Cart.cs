namespace ShopSphere.Domain.Entities;

public sealed class Cart : Entity
{
    private readonly List<CartItem> _items = [];

    private Cart()
    {
    }

    public Cart(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid CustomerId { get; private set; }
    public Guid? CouponId { get; private set; }

    public Coupon? Coupon { get; private set; }

    public decimal DiscountAmount { get; private set; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public decimal SubTotal =>
    _items.Sum(x => x.Subtotal);

    public decimal Total =>
        Math.Max(0, SubTotal - DiscountAmount);

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var existing = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new CartItem(
            productId,
            quantity,
            unitPrice));

        RefreshCoupon();
    }

    public void UpdateQuantity(
         Guid itemId,
         int quantity)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId)
            ?? throw new InvalidOperationException("Cart item not found.");

        item.UpdateQuantity(quantity);

        RefreshCoupon();
    }

    private void RecalculateDiscount(
     Coupon coupon)
    {
        DiscountAmount = coupon.CalculateDiscount(SubTotal);
    }

    public void CompleteCheckout()
    {
        Coupon?.IncrementUsage();
        RemoveCoupon();
    }

    public void RefreshCoupon()
    {
        if (Coupon is null)
        {
            DiscountAmount = 0;
            return;
        }

        RecalculateDiscount(Coupon);
    }

    public CartItem? RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return null;
        _items.Remove(item);
        RefreshCoupon();
        return item; // caller decides what to do with it
    }

    public void ApplyCoupon(
        Coupon coupon)
    {
        Coupon = coupon;
        CouponId = coupon.Id;

        RecalculateDiscount(coupon);
    }

    public void RemoveCoupon()
    {
        Coupon = null;
        CouponId = null;

        DiscountAmount = 0;
    }
    public void Clear()
    {
        _items.Clear();
        RefreshCoupon();
    }
}