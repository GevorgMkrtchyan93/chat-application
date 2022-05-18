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
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var messageList = await _messageService.GetCacheData();

            return View(messageList);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string text)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return NoContent();
                }

                await _messageService.AddDbData(User, text);

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
