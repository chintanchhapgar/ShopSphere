using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ShopSphere.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(
        this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            var body = Expression.Equal(
                Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(bool) },
                    parameter,
                    Expression.Constant(nameof(AuditableEntity.IsDeleted))),
                Expression.Constant(false));

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(Expression.Lambda(body, parameter));
        }
    }
}