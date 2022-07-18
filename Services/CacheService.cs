using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Services
{

    public interface ICacheService
    {
        Task<T> GetCache<T>(string key);
        void SetDefautCache<T>(string key, T data);
        void DeleteKey(string key);
    }
    public class CacheService: ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T> GetCache<T>(string key)
        {

            var value = await _cache.GetAsync(key);
            if (value != null)
            {
                var dataSerialized = Encoding.UTF8.GetString(value);
                var data = JsonConvert.DeserializeObject<T>(dataSerialized);
                return data;
            }
            return default(T);
        }
        public async void SetDefautCache<T>(string key, T data)
        {
            var dataSerialize = JsonConvert.SerializeObject(data);
            var dataToRedis = Encoding.UTF8.GetBytes(dataSerialize);
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(120))
                .SetSlidingExpiration(TimeSpan.FromMinutes(60));
            await _cache.SetAsync(key, dataToRedis, options);
        }
        public void DeleteKey(string key)
        {
            _cache.Remove(key);
        }
    }
}
