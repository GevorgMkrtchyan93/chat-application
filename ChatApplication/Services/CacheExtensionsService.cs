using Abp.Json;
using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.Services
{
    public class CacheExtensionsService : ICacheExtensionsService
    {
        IDistributedCache _distributedCache;

        public CacheExtensionsService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IEnumerable<Message>> GetCacheValueAsync( string key)
        {
            var result = await _distributedCache.GetStringAsync(key);

            if (String.IsNullOrEmpty(result))
            {
                return null;
            }

            var messageCache = JsonConvert.DeserializeObject<List<Message>>(result);
            return messageCache;
        }

        public async Task SetCacheValueAsync<Message>( string key, Message message)
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();

            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120);

            var messageCache = new List<Message>();

            var result = await _distributedCache.GetStringAsync(key);

            if (result != null)
            {
                messageCache = JsonConvert.DeserializeObject<List<Message>>(result);
            }

            messageCache.Add(message);

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(messageCache));
        }

        public async Task SetInitialCacheValueAsync<Message>(string key, List<Message> messages)
        {
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(messages));
        }
    }
}
