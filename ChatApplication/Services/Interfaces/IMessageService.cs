using ChatApplication.Models;
using ChatApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<RedisCacheDataModel>> GetCacheData();
        Task AddDbData(ClaimsPrincipal user, string text);
    }
}
