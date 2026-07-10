using System.Linq.Expressions;

namespace ShopSphere.Application.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue<T>(
        Expression<Func<T, Task>> methodCall);

    string Schedule<T>(
        Expression<Func<T, Task>> methodCall,
        TimeSpan delay);

    void AddOrUpdateRecurring<T>(
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression);
}