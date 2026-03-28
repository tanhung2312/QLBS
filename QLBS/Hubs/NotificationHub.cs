using Microsoft.AspNetCore.SignalR;

namespace QLBS.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinRoom(string userId, string role)
        {
            _logger.LogInformation("[Hub] JoinRoom: userId={UserId}, role={Role}, connectionId={ConnId}",
                userId, role, Context.ConnectionId);

            if (role == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                _logger.LogInformation("[Hub] Đã thêm vào group Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("[Hub] Đã thêm vào group User_{UserId}", userId);
            }
        }
    }
}