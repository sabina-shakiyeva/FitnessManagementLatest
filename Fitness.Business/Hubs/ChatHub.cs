using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
   
    private static ConcurrentDictionary<string, string> _connections = new();

    private static ConcurrentDictionary<string, string> ConnectedGuests = new();

    public override async Task OnConnectedAsync()
    {
        string userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrEmpty(userId))
        {
            _connections[userId] = Context.ConnectionId;

            if (userId.StartsWith("guest-"))
            {
                ConnectedGuests[userId] = Context.ConnectionId;

              
                foreach (var admin in _connections.Where(kv => kv.Key == "admin"))
                {
                    await Clients.Client(admin.Value).SendAsync("NewGuestConnected", userId);
                }
            }

            Console.WriteLine($"User connected: {userId} => {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var disconnectedUser = _connections.FirstOrDefault(kv => kv.Value == Context.ConnectionId).Key;

        if (!string.IsNullOrEmpty(disconnectedUser))
        {
            _connections.TryRemove(disconnectedUser, out _);

            if (disconnectedUser.StartsWith("guest-"))
            {
                ConnectedGuests.TryRemove(disconnectedUser, out _);

                foreach (var admin in _connections.Where(kv => kv.Key == "admin"))
                {
                    await Clients.Client(admin.Value).SendAsync("GuestDisconnected", disconnectedUser);
                }
            }

            Console.WriteLine($"User disconnected: {disconnectedUser}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string toUserId, string message)
    {
        var senderUserId = _connections.FirstOrDefault(kv => kv.Value == Context.ConnectionId).Key ?? "unknown";

        if (_connections.TryGetValue(toUserId, out var receiverConnectionId) && senderUserId != toUserId)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message, senderUserId);
        }


        await Clients.Caller.SendAsync("ReceiveMessage", message, senderUserId);
    }

    public async Task JoinGroup(string userId, string group)
    {
        try
        {
            if (!_connections.TryGetValue(userId, out var connectionId))
            {
                Console.WriteLine($"ConnectionId tapılmadı: {userId}");
                return;
            }

            await Groups.AddToGroupAsync(connectionId, group);
            Console.WriteLine($"{userId} {group} qrupuna qoşulub.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JoinGroup metodu zamanı xəta baş verdi: {ex.Message}");
            throw;
        }
    }
}

