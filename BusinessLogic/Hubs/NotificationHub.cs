using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(object notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task SendToGroup(string groupName, object notification)
    {
        await Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
    }

    public async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task RemoveFromGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
