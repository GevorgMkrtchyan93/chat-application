using ChatApplication.Data;
using ChatApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.Services.Interfaces
{
    public interface ICacheExtensionsService
    {
        Task<IEnumerable<Message>> GetCacheValueAsync( string key);
        Task SetCacheValueAsync<Message>(string key, Message message);
        Task SetInitialCacheValueAsync<Message>(string key, List<Message> messages);
    }
}
