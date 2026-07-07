using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Common;

namespace ShopSphere.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(
        this ModelBuilder modelBuilder)
    {
        var auditableTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(AuditableEntity)
                .IsAssignableFrom(t.ClrType));

        foreach (var entityType in auditableTypes)
        {
            var parameter = Expression.Parameter(
                entityType.ClrType,
                "e");

            var property = Expression.Property(
                parameter,
                nameof(AuditableEntity.IsDeleted));

            var body = Expression.Equal(
                property,
                Expression.Constant(false));

            var lambda = Expression.Lambda(
                body,
                parameter);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(lambda);
        }
    }
}