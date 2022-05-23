﻿using ChatApplication.Data;
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

        public MessageService(ApplicationDbContext context, ICacheExtensionsService cacheExtensionsService)
        {
            _context = context;
            _cacheExtensionsService = cacheExtensionsService;
        }

        public async Task<IEnumerable<RedisCacheDataModel>> GetCacheData()
        {
            var messages = await _cacheExtensionsService.GetCacheValueAsync();
            var messageList = messages.ToList();

            return messageList;
        }

        public async Task AddDbData(AppUser user, string text)
        {
            var dbMessage = new Message();
            dbMessage.Text = text;
            dbMessage.UserId = user.Id;

            await _context.Messages.AddAsync(dbMessage);
            await _context.SaveChangesAsync();

            var message = new RedisCacheDataModel();
            message.Text = text;
            message.UserName = user.UserName;
            message.When = dbMessage.When;

            await _cacheExtensionsService.SetCacheValueAsync(dbMessage);
        }
    }
}
