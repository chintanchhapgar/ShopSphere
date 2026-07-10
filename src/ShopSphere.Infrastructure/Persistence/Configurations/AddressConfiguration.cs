using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class AddressConfiguration
    : IEntityTypeConfiguration<Address>
{
    public void Configure(
        EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(250);

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.IsDefault
        });
    }
}