using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SchoolMS.API.Hubs;

[Authorize]
public class ApiChatHub : Hub
{
    public async Task SendMessage(string roomId, string senderName, string message, string? fileUrl, string? fileType)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMessage", senderName, message, fileUrl, fileType, DateTime.UtcNow);
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
public class ApiNotificationHub : Hub
{
    public async Task SendNotification(string userId, string title, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", title, message, DateTime.UtcNow);
    }
}

[Authorize]
public class ApiLiveStreamChatHub : Hub
{
    public async Task JoinLiveStream(string liveStreamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"livestream-{liveStreamId}");
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.Group($"livestream-{liveStreamId}").SendAsync("UserJoinedStream", userName);
    }

    public async Task LeaveLiveStream(string liveStreamId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"livestream-{liveStreamId}");
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.Group($"livestream-{liveStreamId}").SendAsync("UserLeftStream", userName);
    }

    public async Task SendLiveComment(string liveStreamId, string studentName, string senderType, string comment)
    {
        await Clients.Group($"livestream-{liveStreamId}")
            .SendAsync("ReceiveLiveComment", studentName, senderType, comment, DateTime.UtcNow);
    }
}
