using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    $"ShopSphere-{Guid.NewGuid()}");
            });

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
        });
    }
}