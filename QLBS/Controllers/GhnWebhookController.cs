using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QLBS.Constants;
using QLBS.Dtos.Ghn;
using QLBS.Hubs;
using QLBS.Models;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GhnWebhookController : ControllerBase
    {
        private readonly QLBSDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<GhnWebhookController> _logger;

        public GhnWebhookController(
            QLBSDbContext context,
            IHubContext<NotificationHub> hubContext,
            ILogger<GhnWebhookController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] GhnWebhookDto dto)
        {
            try
            {
                if (dto.Status.ToLower() == "delivered" || dto.Status.ToLower() == "finish")
                {
                    var order = await _context.OrderTables
                        .Include(o => o.OrderDetails)
                        .FirstOrDefaultAsync(o => o.GhnOrderCode == dto.OrderCode);

                    if (order != null && order.OrderStatus != OrderStatusConstants.Completed)
                    {
                        order.OrderStatus = OrderStatusConstants.Completed;

                        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);
                        if (payment != null && payment.PaymentMethodId == PaymentMethodConstants.COD)
                        {
                            payment.PaymentStatus = PaymentStatusConstants.Success;
                            payment.PaymentDate = DateTime.Now;
                        }

                        await _context.SaveChangesAsync();

                        var bookIds = order.OrderDetails.Select(d => d.BookId).ToList();

                        await _hubContext.Clients.Group($"User_{order.UserId}").SendAsync("ReceiveOrderStatus", new
                        {
                            OrderId = order.OrderId,
                            NewStatus = "Hoàn thành",
                            Message = $"Đơn hàng #{order.OrderId} đã giao thành công. Bạn hãy đánh giá nhé!",
                            Timestamp = DateTime.Now,
                            CanReviewBookIds = bookIds
                        });

                        await _hubContext.Clients.Group("Admins").SendAsync("AdminOrderUpdate", new
                        {
                            OrderId = order.OrderId,
                            NewStatus = "Hoàn thành",
                            Message = $"Đơn GHN #{dto.OrderCode} đã giao xong.",
                            Timestamp = DateTime.Now
                        });

                        _logger.LogInformation($"Order {order.OrderId} completed via Webhook.");
                    }
                }
                return Ok(new { message = "Received" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHN Webhook Error");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }
    }
}