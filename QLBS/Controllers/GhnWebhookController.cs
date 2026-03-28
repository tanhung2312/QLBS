using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
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
                _logger.LogInformation("[GHN Webhook] OrderCode={Code} Status={Status}",
                    dto.OrderCode, dto.Status);

                if (string.IsNullOrWhiteSpace(dto.OrderCode) || string.IsNullOrWhiteSpace(dto.Status))
                    return BadRequest(new { message = "OrderCode hoặc Status không được để trống." });

                var order = await _context.OrderTables
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.GhnOrderCode == dto.OrderCode);

                if (order == null)
                {
                    _logger.LogWarning("[GHN Webhook] Không tìm thấy đơn: {Code}", dto.OrderCode);
                    return Ok(new { message = "Order not found, ignored" });
                }


                byte? newStatus = dto.Status.ToLower() switch
                {
                    "ready_to_pick" => OrderStatusConstants.Processing, 
                    "picking" => OrderStatusConstants.Processing,  
                    "picked" => OrderStatusConstants.Processing, 
                    "storing" => OrderStatusConstants.Processing,  
                    "transporting" => OrderStatusConstants.Shipping,    
                    "sorting" => OrderStatusConstants.Shipping,    
                    "delivering" => OrderStatusConstants.Shipping,    
                    "delivered" => OrderStatusConstants.Completed,   
                    "finish" => OrderStatusConstants.Completed,   
                    "cancel" => OrderStatusConstants.Cancelled,   
                    "return" => OrderStatusConstants.Cancelled,   
                    "returned" => OrderStatusConstants.Cancelled,   
                    "exception" => OrderStatusConstants.Cancelled,   
                    "damage" => OrderStatusConstants.Cancelled,   
                    "lost" => OrderStatusConstants.Cancelled,   
                    _ => null
                };

                if (newStatus == null)
                {
                    _logger.LogInformation("[GHN Webhook] Bỏ qua status không xử lý: {Status}", dto.Status);
                    return Ok(new { message = "Status ignored" });
                }

                if (order.OrderStatus == OrderStatusConstants.Completed ||
                    order.OrderStatus == OrderStatusConstants.Cancelled)
                {
                    _logger.LogInformation("[GHN Webhook] Đơn {Id} đã ở trạng thái cuối, bỏ qua.", order.OrderId);
                    return Ok(new { message = "Final status, ignored" });
                }

                order.OrderStatus = newStatus.Value;

                if (newStatus == OrderStatusConstants.Completed)
                {
                    var payment = order.Payments.FirstOrDefault();
                    if (payment != null && payment.PaymentMethodId == PaymentMethodConstants.COD)
                    {
                        payment.PaymentStatus = PaymentStatusConstants.Success;
                        payment.PaymentDate = DateTime.Now;
                        _logger.LogInformation("[GHN Webhook] COD payment completed for Order {Id}", order.OrderId);
                    }
                }

                await _context.SaveChangesAsync();

                string statusName = newStatus.Value switch
                {
                    OrderStatusConstants.Processing => "Đang xử lý",
                    OrderStatusConstants.Shipping => "Đang giao hàng",
                    OrderStatusConstants.Completed => "Giao thành công",
                    OrderStatusConstants.Cancelled => "Đã hủy / Hoàn hàng",
                    _ => "Cập nhật trạng thái"
                };

                string userMessage = newStatus.Value switch
                {
                    OrderStatusConstants.Processing => $"Đơn hàng #{order.OrderId} đang được chuẩn bị.",
                    OrderStatusConstants.Shipping => $"Đơn hàng #{order.OrderId} đang trên đường giao đến bạn.",
                    OrderStatusConstants.Completed => $"Đơn hàng #{order.OrderId} đã giao thành công! Hãy đánh giá sản phẩm nhé.",
                    OrderStatusConstants.Cancelled => $"Đơn hàng #{order.OrderId} đã bị hủy hoặc đang hoàn hàng.",
                    _ => $"Đơn hàng #{order.OrderId} cập nhật trạng thái: {statusName}."
                };

                await _hubContext.Clients
                    .Group($"User_{order.UserId}")
                    .SendAsync("ReceiveOrderStatus", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,
                        Message = userMessage,
                        Timestamp = DateTime.Now,
                        CanReviewBookIds = newStatus == OrderStatusConstants.Completed
                            ? order.OrderDetails.Select(d => d.BookId).ToList()
                            : new List<int>()
                    });

                await _hubContext.Clients
                    .Group("Admins")
                    .SendAsync("AdminOrderUpdate", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,
                        Message = $"GHN #{dto.OrderCode} → {statusName}",
                        Timestamp = DateTime.Now
                    });

                _logger.LogInformation("[GHN Webhook] Order {Id} updated → {Status}",
                    order.OrderId, statusName);

                return Ok(new { message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GHN Webhook] Lỗi xử lý webhook");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpPost("test-update/{orderCode}/{status}")]
        public async Task<IActionResult> TestUpdate(string orderCode, string status)
        {
            _logger.LogInformation("[GHN Test] Simulate webhook: {Code} → {Status}",
                orderCode, status);

            return await UpdateStatus(new GhnWebhookDto
            {
                OrderCode = orderCode,
                Status = status
            });
        }

        [HttpGet("check/{orderCode}")]
        public async Task<IActionResult> CheckOrder(string orderCode)
        {
            var order = await _context.OrderTables
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.GhnOrderCode == orderCode);

            if (order == null)
                return NotFound(new { message = $"Không tìm thấy đơn với GHN code: {orderCode}" });

            var payment = order.Payments.FirstOrDefault();

            return Ok(new
            {
                orderId = order.OrderId,
                ghnOrderCode = order.GhnOrderCode,
                orderStatus = order.OrderStatus,
                statusName = order.OrderStatus switch
                {
                    OrderStatusConstants.Pending => "Chờ xử lý",
                    OrderStatusConstants.Processing => "Đang xử lý",
                    OrderStatusConstants.Confirmed => "Đã xác nhận",
                    OrderStatusConstants.Shipping => "Đang giao hàng",
                    OrderStatusConstants.Completed => "Hoàn thành",
                    OrderStatusConstants.Cancelled => "Đã hủy",
                    _ => "Không xác định"
                },
                paymentStatus = payment?.PaymentStatus,
                paymentMethod = payment?.PaymentMethodId,
                receiverName = order.ReceiverName,
                totalAmount = order.TotalAmount,
                orderDate = order.OrderDate
            });
        }
    }
}