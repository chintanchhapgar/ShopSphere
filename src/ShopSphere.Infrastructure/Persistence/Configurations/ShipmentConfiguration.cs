using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class ShipmentConfiguration
    : IEntityTypeConfiguration<Shipment>
{
    public void Configure(
        EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Carrier)
            .HasMaxLength(100);

        builder.HasOne(x => x.Order)
            .WithOne(x => x.Shipment)
            .HasForeignKey<Shipment>(
                x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}