using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QLBS.Constants;
using QLBS.Dtos.Order;
using QLBS.Hubs;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;
using System.Security.Claims;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<OrderController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IOrderRepository _orderRepository;

        public OrderController(
            IOrderService orderService,
            IVnPayService vnPayService,
            ILogger<OrderController> logger,
            IHubContext<NotificationHub> hubContext,
            IOrderRepository orderRepository)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
            _logger = logger;
            _hubContext = hubContext;
            _orderRepository = orderRepository;
        }

        // ── User: Tạo đơn hàng ───────────────────────────────────────────────
        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId))
                    return Unauthorized();

                var result = await _orderService.CreateOrderAsync(accountId, dto, HttpContext);
                return Ok(result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateException: {Inner}", dbEx.InnerException?.Message);
                return StatusCode(500, new { message = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout Error: {Inner}", ex.InnerException?.Message);
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ── User: Lịch sử đơn hàng ───────────────────────────────────────────
        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId))
                    return Unauthorized();

                var result = await _orderService.GetOrderHistoryAsync(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get History Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        // ── User: Chi tiết đơn hàng (chỉ xem đơn của mình) ──────────────────
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId))
                    return Unauthorized();

                var result = await _orderService.GetOrderDetailAsync(accountId, id);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get Order Detail Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        // ── Admin: Lấy tất cả đơn hàng ───────────────────────────────────────
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var result = await _orderService.GetAllOrdersForAdminAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin GetAllOrders Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        // ── Admin: Chi tiết đơn hàng bất kỳ ─────────────────────────────────
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDetail(int id)
        {
            try
            {
                var result = await _orderService.GetOrderDetailForAdminAsync(id);
                if (result is null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin GetDetail Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        // ── Admin: Cập nhật trạng thái đơn ───────────────────────────────────
        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] byte newStatus)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });

                if (order.OrderStatus == OrderStatusConstants.Cancelled)
                    return BadRequest(new { message = "Không thể cập nhật đơn hàng đã bị hủy." });

                if (order.OrderStatus == OrderStatusConstants.Completed)
                    return BadRequest(new { message = "Không thể cập nhật đơn hàng đã hoàn thành." });

                await _orderRepository.UpdateOrderStatusAsync(id, newStatus);

                string statusName = GetOrderStatusName(newStatus); // ✅ thêm dòng này

                string statusMessage = newStatus switch
                {
                    OrderStatusConstants.Pending => "Đơn hàng đang chờ xử lý.",
                    OrderStatusConstants.Processing => "Đơn hàng đang được đóng gói.",
                    OrderStatusConstants.Confirmed => "Đơn hàng đã được xác nhận.",
                    OrderStatusConstants.Shipping => "Đơn hàng đang được giao.",
                    OrderStatusConstants.Completed => "Đơn hàng đã giao thành công.",
                    OrderStatusConstants.Cancelled => "Đơn hàng đã bị hủy bởi Admin.",
                    _ => "Trạng thái đơn hàng đã thay đổi."
                };

                _logger.LogInformation("[SignalR] Gửi tới Group: User_{UserId}, OrderId={OrderId}, Status={Status}",
                    order.UserId, order.OrderId, statusName);

                // Notify user
                await _hubContext.Clients
                    .Group($"User_{order.UserId}")
                    .SendAsync("ReceiveOrderStatus", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,  // ✅ string thay vì byte
                        Message = statusMessage,
                        Timestamp = DateTime.Now
                    });

                // Notify admins
                await _hubContext.Clients
                    .Group("Admins")
                    .SendAsync("AdminOrderUpdate", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,  // ✅ string thay vì byte
                        Message = $"Admin vừa cập nhật đơn #{id} → {statusName}"
                    });

                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Status Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }


        [HttpPost("simulate-ghn/{orderId}/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SimulateGhn(int orderId, string status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });

                if (string.IsNullOrEmpty(order.GhnOrderCode))
                    return BadRequest(new { message = "Đơn hàng chưa có mã GHN." });

                // Gọi thẳng GhnWebhookController để xử lý giống webhook thật
                var webhookDto = new QLBS.Dtos.Ghn.GhnWebhookDto
                {
                    OrderCode = order.GhnOrderCode,
                    Status = status
                };

                // Map status → byte
                byte? newStatus = status.ToLower() switch
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
                    _ => null
                };

                if (newStatus == null)
                    return BadRequest(new
                    {
                        message = $"Status '{status}' không hợp lệ.",
                        validStatuses = new[]
                        {
                            "ready_to_pick", "picking", "picked",
                            "delivering", "delivered", "finish",
                            "cancel", "return"
                        }
                    });

                if (order.OrderStatus == OrderStatusConstants.Completed ||
                    order.OrderStatus == OrderStatusConstants.Cancelled)
                    return BadRequest(new { message = "Đơn đã ở trạng thái cuối, không thể cập nhật." });

                // Cập nhật trạng thái đơn
                order.OrderStatus = newStatus.Value;

                // COD giao thành công → cập nhật thanh toán
                if (newStatus == OrderStatusConstants.Completed)
                {
                    var payment = order.Payments.FirstOrDefault();
                    if (payment != null && payment.PaymentMethodId == PaymentMethodConstants.COD)
                    {
                        await _orderRepository.UpdatePaymentStatusAsync(
                            orderId, PaymentStatusConstants.Success, $"SIM_{orderId}_{DateTime.Now:yyyyMMddHHmmss}");
                    }
                }

                await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus.Value);

                string statusName = GetOrderStatusName(newStatus.Value);

                // Notify user qua SignalR
                await _hubContext.Clients
                    .Group($"User_{order.UserId}")
                    .SendAsync("ReceiveOrderStatus", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,
                        Message = $"[TEST] Đơn hàng #{orderId}: {statusName}",
                        Timestamp = DateTime.Now
                    });

                // Notify admin
                await _hubContext.Clients
                    .Group("Admins")
                    .SendAsync("AdminOrderUpdate", new
                    {
                        OrderId = order.OrderId,
                        NewStatus = statusName,
                        Message = $"[Simulate GHN] Đơn #{orderId} → {statusName}",
                        Timestamp = DateTime.Now
                    });

                _logger.LogInformation("[Simulate GHN] Order {Id} → {Status}", orderId, statusName);

                return Ok(new
                {
                    message = $"Đã cập nhật đơn #{orderId} → {statusName}",
                    orderId = orderId,
                    ghnCode = order.GhnOrderCode,
                    ghnStatus = status,
                    systemStatus = statusName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Simulate GHN Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private static string GetOrderStatusName(byte status) => status switch
        {
            OrderStatusConstants.Pending => "Chờ xử lý",
            OrderStatusConstants.Processing => "Đang đóng gói",
            OrderStatusConstants.Confirmed => "Đã xác nhận",
            OrderStatusConstants.Shipping => "Đang giao hàng",
            OrderStatusConstants.Completed => "Hoàn thành",
            OrderStatusConstants.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }
}