using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class CouponConfiguration
    : IEntityTypeConfiguration<Coupon>
{
    public void Configure(
        EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.DiscountType)
            .HasConversion<int>();

        builder.Property(x => x.DiscountValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.MinimumOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaximumDiscountAmount)
            .HasPrecision(18, 2);
    }
}