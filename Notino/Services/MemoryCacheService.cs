using Microsoft.Extensions.Caching.Memory;
using Notino.Interfaces;

namespace Notino.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public readonly TimeSpan DefaultEntryExpiration = TimeSpan.FromMinutes(10);


        public MemoryCacheService(IMemoryCache cache)
        {
            _memoryCache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out T value) ? value : default(T));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, TimeSpan? slidingExpiration = null)
        {
            if (expiration == null)
                expiration = DefaultEntryExpiration;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expiration,
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set<T>(key, value, cacheEntryOptions);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache?.Remove(key);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
    }
}
