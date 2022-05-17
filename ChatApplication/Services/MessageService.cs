using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Services.Interfaces;
using ChatApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Services
{
    public class MessageService : IMessageService
    {
        private readonly ICacheExtensionsService _cacheExtensionsService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageService(ApplicationDbContext context, ICacheExtensionsService cacheExtensionsService, UserManager<AppUser> userManager)
        {
            _context = context;
            _cacheExtensionsService = cacheExtensionsService;
            _userManager = userManager;
        }

        public async Task<IEnumerable<RedisCacheDataModel>> GetCacheData()
        {
            var messages = await _cacheExtensionsService.GetCacheValueAsync();
            var messageList = messages.ToList();

            return messageList;
        }

        public async Task AddDbData(ClaimsPrincipal user, string text)
        {
            var sender = await _userManager.GetUserAsync(user);

            var dbMessage = new Message();
            dbMessage.Text = text;
            dbMessage.UserId = sender.Id;
            await _context.Messages.AddAsync(dbMessage);
            await _context.SaveChangesAsync();

            var message = new RedisCacheDataModel();
            message.Text = text;
            message.UserName = sender.UserName;
            await _cacheExtensionsService.SetCacheValueAsync(dbMessage);
        }
    }
}
