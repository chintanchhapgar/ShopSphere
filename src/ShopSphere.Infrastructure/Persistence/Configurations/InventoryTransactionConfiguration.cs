using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class InventoryTransactionConfiguration
    : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(
        EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.TransactionType)
            .HasConversion<int>();

        builder.Property(x => x.Reason)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(100);

        builder.HasOne(x => x.Inventory)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InventoryId);

        builder.HasIndex(x => x.CreatedAtUtc);
    }
}