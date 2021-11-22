using Microsoft.Extensions.Caching.Memory;
using System;

namespace Example.Covid19.API.Services
{
    public class Covid19MemoryCacheService : ICovid19MemoryCacheService
    {
        protected readonly IMemoryCache memoryCache;

        public Covid19MemoryCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public bool Get<T>(string key, out T entry) where T : class
        {
            return memoryCache.TryGetValue(key, out entry);
        }

        public void Set<T>(string key, T entry) where T : class
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(12));

            memoryCache.Set(key, entry, cacheEntryOptions);
        }
    }
}
