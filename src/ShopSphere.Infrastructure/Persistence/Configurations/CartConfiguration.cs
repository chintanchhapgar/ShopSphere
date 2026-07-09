using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
        .HasOne(x => x.Coupon)
        .WithMany()
        .HasForeignKey(x => x.CouponId)
        .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.CustomerId)
            .IsUnique();
    }
}