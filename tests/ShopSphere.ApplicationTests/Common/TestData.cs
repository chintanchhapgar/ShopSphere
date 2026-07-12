using ShopSphere.Domain.Entities;
using System.Reflection;

namespace ShopSphere.ApplicationTests.Common;

public static class TestData
{
    public static Category CreateCategory()
    {
        return new Category(
            "Electronics",
            "Electronic products",
            null);
    }

    public static Brand CreateBrand()
    {
        return new Brand(
            "Apple",
            "Apple Brand");
    }

    public static Product CreateProduct(
    Guid categoryId,
    Guid brandId)
    {
        var product = new Product(
            "iPhone 16",
            "Latest iPhone",
            "IPHONE-001",
            1000m,
            null,
            categoryId,
            brandId,
            "iphone-16",
            null,
            null);

        var imagesField = typeof(Product)
            .GetField("_images",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        imagesField?.SetValue(
            product,
            new List<ProductImage>());

        return product;
    }

    public static Cart CreateCart(
    Guid customerId,
    Product product,
    int quantity = 2)
    {
        var cart = new Cart(customerId);

        cart.AddItem(
            product.Id,
            quantity,
            product.BasePrice);

        var cartItem = cart.Items.Single();

        typeof(CartItem)
            .GetProperty(nameof(CartItem.Product))!
            .SetValue(cartItem, product);

        return cart;
    }
    public static Order CreateOrder(Guid userId)
    {
        var order = Order.Create(
            userId,
            "ORD-0001",
            "John Doe",
            "9876543210",
            "123 Main Street",
            null,
            "Surat",
            "Gujarat",
            "395001",
            "India");

        var productId = Guid.NewGuid();

        order.AddItem(
            OrderItem.Create(
                productId,
                "iPhone 16",
                "IPH-001",
                null,
                1000m,
                2));

        return order;
    }

}