using ChatService.API.DataService;
using ChatService.Repository.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDB _shared;

        public ChatHub(SharedDB shared)
        {
            _shared = shared;
        }

        public async Task JoinChat(UserConnection conn)
        {
            await Clients.All
                .SendAsync("JoinChat", "admin", $"{conn.Username} has joined!");
        }

        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);

            _shared.connections[Context.ConnectionId] = conn;

            await Clients.Group(conn.ChatRoom)
                .SendAsync("ReceiveJoinSpecificChatRoomMessage", "admin", $"{conn.Username} has joined {conn.ChatRoom}!");
        }

        public async Task SendMessage(string msg)
        {
            if (_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom)
                    .SendAsync("ReceiveSpecificMessage", conn.Username, msg);

            }
        }
    }
}
