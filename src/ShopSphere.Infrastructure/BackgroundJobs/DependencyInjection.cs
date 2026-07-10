using Microsoft.Extensions.DependencyInjection;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.BackgroundJobs.Jobs;

namespace ShopSphere.Infrastructure.BackgroundJobs;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services)
    {
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

        services.AddScoped<IEmailJob, EmailJob>();
        services.AddScoped<InventoryJob>();
        services.AddScoped<OrderCleanupJob>();
        services.AddScoped<ReviewReminderJob>();

        return services;
    }
}