using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Services.Interfaces
{
    public interface ICacheExtensionsService
    {
        Task<IEnumerable<RedisCacheDataModel>> GetCacheValueAsync();
        Task SetCacheValueAsync(Message message);
        Task SetInitialCacheValueAsync(List<RedisCacheDataModel> messages);
    }
}
