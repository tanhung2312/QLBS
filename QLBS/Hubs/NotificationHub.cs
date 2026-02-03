using Microsoft.AspNetCore.SignalR;

namespace QLBS.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinRoom(string userId, string role)
        {
            if (role == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
        }
    }
}