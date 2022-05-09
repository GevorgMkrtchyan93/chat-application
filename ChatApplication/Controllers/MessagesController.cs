using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
<<<<<<< HEAD
using Microsoft.Extensions.Options;
=======
>>>>>>> 6bc8348db9b4805d2e387f7e5b56dde68ac1cd7a
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
        private readonly IDistributedCache _distributedCache;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
<<<<<<< HEAD
        private readonly ICacheExtensionsService _cacheExtensionsService;

        string cacheKey = "Allmessages0";

        public MessagesController(ApplicationDbContext context, UserManager<AppUser> userManager, ICacheExtensionsService cacheExtensionsService)
=======


        public MessagesController(ApplicationDbContext context, UserManager<AppUser> userManager, IDistributedCache distributedCache)
>>>>>>> 6bc8348db9b4805d2e387f7e5b56dde68ac1cd7a
        {
            _distributedCache = distributedCache;
            _context = context;
            _userManager = userManager;
            _cacheExtensionsService = cacheExtensionsService;
        }

<<<<<<< HEAD
        [HttpGet]
=======
        [HttpGet("Messages/Redis")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllMessagesUsingRedisCache()
        {
            var cacheKey = "Allmessages";
            string serializedMessageList;
            var messageList = new List<Message>();
            var redisMessageList = await _distributedCache.GetAsync(cacheKey);
            if (redisMessageList != null)
            {
                serializedMessageList = Encoding.UTF8.GetString(redisMessageList);
                messageList = JsonConvert.DeserializeObject<List<Message>>(serializedMessageList);
            }
            else
            {
                messageList = await _context.Messages.ToListAsync();
                serializedMessageList = JsonConvert.SerializeObject(messageList);
                redisMessageList = Encoding.UTF8.GetBytes(serializedMessageList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(2));
                await _distributedCache.SetAsync(cacheKey, redisMessageList, options);
            }
            return Ok(messageList);
        }

>>>>>>> 6bc8348db9b4805d2e387f7e5b56dde68ac1cd7a
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
