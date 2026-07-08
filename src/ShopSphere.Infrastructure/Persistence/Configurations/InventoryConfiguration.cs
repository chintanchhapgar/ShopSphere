using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class InventoryConfiguration
    : IEntityTypeConfiguration<Inventory>
{
    public void Configure(
        EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityOnHand)
            .IsRequired();

        builder.Property(x => x.ReservedQuantity)
            .IsRequired();

        builder.Property(x => x.LowStockThreshold)
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithOne(x => x.Inventory)
            .HasForeignKey<Inventory>(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProductId)
            .IsUnique();
    }
}