using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache) => _cache = cache;

        public async Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<Task<T>> retrieveDataFunc,
        TimeSpan? slidingExpiration = null)
        {
            if (!_cache.TryGetValue(cacheKey, out T cachedData))
            {
                // Data not in cache, retrieve it
                cachedData = await retrieveDataFunc();

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(60)
                };

                // Save data in cache
                _cache.Set(cacheKey, cachedData, cacheEntryOptions);
            }

            return cachedData;
        }

    }
}
