using ChatApplication.Data;
using ChatApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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


        public MessagesController(ApplicationDbContext context, UserManager<AppUser> userManager, IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _context = context;
            _userManager = userManager;
        }

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

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = await _context.Messages.ToListAsync();
            messages.Count();
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
