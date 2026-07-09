using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(
        EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.Method)
            .HasConversion<int>();

        builder.Property(x => x.TransactionId)
            .HasMaxLength(200);

        builder.HasOne(x => x.Order)
        .WithOne(x => x.Payment)
        .HasForeignKey<Payment>(
            x => x.OrderId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}