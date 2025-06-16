using TransitGtfs.Api.Interfaces.Database;
using TransitGtfs.Api.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace TransitGtfs.Api.Service.Database
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisService> _logger;
        private readonly TimeSpan _duration;

        public RedisService(IDistributedCache cache, ILogger<RedisService> logger, TimeSpan? duration = null)
        {
            _cache = cache;
            _logger = logger;
            _duration = duration ?? Constant.CacheDuration;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory) where T : class
        {
            try
            {
                key = key.ToLower();
                string? data = await _cache.GetStringAsync(key);

                if (!string.IsNullOrEmpty(data))
                {
                    _logger.LogDebug($"[Redis] Cache hit for key: {key}");
                    return await JsonUtil.StringToObjectAsync<T>(data);
                }

                _logger.LogDebug($"[Redis] Cache miss for key: {key}");

                T result = await factory();

                if (result != null)
                {
                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _duration
                    };

                    string? json = await JsonUtil.ObjectToStringAsync<T>(result);

                    if (!string.IsNullOrEmpty(json) && !json.Equals("[]"))
                    {
                        await _cache.SetStringAsync(key, json, options);
                        _logger.LogDebug($"[Redis] Item cached with key: {key}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Redis] Error getting or setting item in cache with key: {key}");
                return await factory();
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                key = key.ToLower();
                await _cache.RemoveAsync(key);
                _logger.LogDebug("[Redis] Removed item from cache with key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Redis] Error removing item from cache with key: {Key}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            _logger.LogWarning($"{nameof(RemoveByPrefixAsync)} not fully implemented for Redis. Prefix: {prefix}");
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                key = key.ToLower();
                string? data = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Redis] Error checking if key exists in cache: {key}");
                return false;
            }
        }
    }
}