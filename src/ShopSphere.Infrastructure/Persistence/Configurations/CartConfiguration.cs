using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class CartConfiguration
    : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Items)
     .WithOne(x => x.Cart)
     .HasForeignKey(x => x.CartId)
     .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Cart.Items))!
            .SetField("_items");

        builder.HasIndex(x => x.CustomerId)
            .IsUnique();
    }
}