using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Authentication;
using ShopSphere.Infrastructure.Identity;
using ShopSphere.Infrastructure.Persistence;
using ShopSphere.Infrastructure.Persistence.Interceptors;
using ShopSphere.Infrastructure.Persistence.Repositories;
using ShopSphere.Infrastructure.Queries;
using ShopSphere.Infrastructure.Services;
using ShopSphere.Infrastructure.Storage;
using System.Text;

namespace ShopSphere.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<ApplicationRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<ITokenProvider, JwtTokenProvider>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IFileValidationService, FileValidationService>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IOrderFulfillmentService, OrderFulfillmentService>();

        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        // Queries
        services.AddScoped<ICategoryQueries, CategoryQueries>();
        services.AddScoped<IBrandQueries, BrandQueries>();
        services.AddScoped<IProductQueries, ProductQueries>();
        services.AddScoped<IInventoryQueries, InventoryQueries>();
        services.AddScoped<IInventoryTransactionQueries, InventoryTransactionQueries>();
        services.AddScoped<ICartQueries, CartQueries>();
        services.AddScoped<IOrderQueries, OrderQueries>();
        services.AddScoped<IPaymentQueries, PaymentQueries>();
        services.AddScoped<IShipmentQueries, ShipmentQueries>();

        services.Configure<FileStorageOptions>(
            configuration.GetSection(FileStorageOptions.SectionName));

        services.AddScoped<IApplicationDbContext>(
            sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<AuditableEntityInterceptor>();

        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<AuditableEntityInterceptor>();

        return services;
    }
}