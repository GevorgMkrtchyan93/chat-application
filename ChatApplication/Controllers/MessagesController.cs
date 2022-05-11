using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    //[Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheExtensionsService _cacheExtensionsService;

        string cacheKey = "Allmessages0";

        public MessagesController(ApplicationDbContext context, UserManager<AppUser> userManager, ICacheExtensionsService cacheExtensionsService)

        {
            _distributedCache = distributedCache;
            _context = context;
            _userManager = userManager;
            _cacheExtensionsService = cacheExtensionsService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var messages = await _cacheExtensionsService.GetCacheValueAsync(cacheKey);
            var messageList = messages.ToList();
            return View(messageList);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Message message)
        {
            
            if (ModelState.IsValid)
            {
                if (message.Text == null)
                {
                    return NoContent();
                }
                message.UserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.UserId = sender.Id;

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
               
                await _cacheExtensionsService.SetCacheValueAsync<Message>(cacheKey, message);

                return Ok();
            }
            return Error();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
