using Abp.Json;
using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Services.Interfaces;
using ChatApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Services
{
    public class CacheExtensionsService : ICacheExtensionsService
    {
        public string cacheKey = "Allmessages";
        private readonly IDistributedCache _distributedCache;

        public CacheExtensionsService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IEnumerable<RedisCacheDataModel>> GetCacheValueAsync()
        {
            var result = await _distributedCache.GetStringAsync(cacheKey);

            if (String.IsNullOrEmpty(result))
            {
                return new List<RedisCacheDataModel>();
            }

            var messageCache = JsonConvert.DeserializeObject<List<RedisCacheDataModel>>(result);

            return messageCache;
        }

        public async Task SetCacheValueAsync(Message message)
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120);
            var result = await _distributedCache.GetStringAsync(cacheKey);

            if (String.IsNullOrEmpty(result))
            {
                return;
            }

            var messageList = JsonConvert.DeserializeObject<List<RedisCacheDataModel>>(result);
            messageList.Add(
                new RedisCacheDataModel
                {
                    Text = message.Text,
                    UserName = message.Sender.UserName,
                    When = message.When
                });

            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(messageList));
        }

        public async Task SetInitialCacheValueAsync(List<RedisCacheDataModel> messages)
        {
            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(messages));
        }
    }
}

