using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatService.Service.Services
{
    public interface IRedisService
    {
        T? GetData<T>(string key);
        void SetData<T>(string key, T data, TimeSpan time);
    }
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache? _cache;

        public RedisService(IDistributedCache? cache)
        {
            _cache = cache;
        }
        public T? GetData<T>(string key)
        {
            var data = _cache?.GetString(key);

            if (data is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data, TimeSpan timeSpan)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = timeSpan
            };
            _cache?.SetString(key, JsonSerializer.Serialize(data), options);
        }
    }
}
