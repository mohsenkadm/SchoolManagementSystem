using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SchoolMS.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string roomId, string message)
    {
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.Group(roomId).SendAsync("ReceiveMessage", userName, message, DateTime.UtcNow);
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.Group(roomId).SendAsync("UserJoined", userName);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.Group(roomId).SendAsync("UserLeft", userName);
    }
}

[Authorize]
public class NotificationHub : Hub
{
    public async Task SendNotification(string userId, string title, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", title, message, DateTime.UtcNow);
    }

    [Authorize(Roles = "SuperAdmin,SchoolAdmin")]
    public async Task BroadcastNotification(string title, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", title, message, DateTime.UtcNow);
    }
}
