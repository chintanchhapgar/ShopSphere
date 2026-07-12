using Moq;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.ApplicationTests.Common;

public abstract class HandlerTestBase
{
    protected readonly Mock<ICategoryRepository> CategoryRepository = new();

    protected readonly Mock<IBrandRepository> BrandRepository = new();

    protected readonly Mock<IProductRepository> ProductRepository = new();

    protected readonly Mock<IOrderRepository> OrderRepository = new();

    protected readonly Mock<ICouponRepository> CouponRepository = new();

    protected readonly Mock<IReviewRepository> ReviewRepository = new();

    protected readonly Mock<IWishlistRepository> WishlistRepository = new();

    protected readonly Mock<IInventoryRepository> InventoryRepository = new();

    protected readonly Mock<ICartRepository> CartRepository = new();

    protected readonly Mock<IPaymentRepository> PaymentRepository = new();

    protected readonly Mock<IShipmentRepository> ShipmentRepository = new();

    protected readonly Mock<ICacheService> CacheService = new();

    protected readonly Mock<IAuditService> AuditService = new();

    protected readonly Mock<ICurrentUserService> CurrentUserService = new();

    protected readonly Mock<IEmailService> EmailService = new();

    //protected readonly Mock<IJwtTokenGenerator> JwtTokenGenerator = new();

    protected readonly Mock<IFileStorageService> FileStorageService = new();
}