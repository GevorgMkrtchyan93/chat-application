using ChatApplication.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApplication.Hubs
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("GetDisconnectionUsersCount");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task NewConnection()
        {
            await Clients.All.SendAsync("NewConnection", UserHandler.ConnectedIds.Count);
        }

        public async Task GetOnlineUsersCount()
        {
            await Clients.Caller.SendAsync("GetOnlineUsersCount", UserHandler.ConnectedIds.Count);
        }

        public async Task GetDisconnectionUsersCount()
        {
            await Clients.Caller.SendAsync("GetDisconnectionUsersCount", UserHandler.ConnectedIds.Count);
        }
    }
}
