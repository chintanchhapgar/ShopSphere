using Microsoft.Extensions.Caching.Distributed;
using ShopSphere.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ShopSphere.Infrastructure.Caching;

public sealed class RedisCacheService
    : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    public RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                expiration ?? TimeSpan.FromMinutes(15)
        };

        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            options,
            cancellationToken);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(
            key,
            cancellationToken);
    }

    public async Task RemoveByPrefixAsync(
    string prefix,
    CancellationToken cancellationToken = default)
    {
        var endpoints = _redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);

            if (!server.IsConnected)
            {
                continue;
            }

            var database = _redis.GetDatabase();

            var keys = server.Keys(
                pattern: $"ShopSphere:{prefix}*");

            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);
            }
        }
    }
}