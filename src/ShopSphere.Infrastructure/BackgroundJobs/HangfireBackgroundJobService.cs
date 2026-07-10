using System.Linq.Expressions;
using Hangfire;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Infrastructure.BackgroundJobs;

public sealed class HangfireBackgroundJobService
    : IBackgroundJobService
{
    public string Enqueue<T>(
        Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule<T>(
        Expression<Func<T, Task>> methodCall,
        TimeSpan delay)
    {
        return BackgroundJob.Schedule(
            methodCall,
            delay);
    }

    public void AddOrUpdateRecurring<T>(
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression)
    {
        RecurringJob.AddOrUpdate(
            recurringJobId,
            methodCall,
            cronExpression);
    }
}