using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(
        EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Entity)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.UserName)
            .HasMaxLength(256);

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => x.Entity);

        builder.HasIndex(x => x.Action);

        builder.HasIndex(x => x.UserId);
    }
}