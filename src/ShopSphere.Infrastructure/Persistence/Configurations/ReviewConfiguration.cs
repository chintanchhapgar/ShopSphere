using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(
        EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Comment)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}