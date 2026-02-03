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

        public OrderController(IOrderService orderService, IVnPayService vnPayService, ILogger<OrderController> logger, IHubContext<NotificationHub> hubContext, IOrderRepository orderRepository)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
            _logger = logger;
            _hubContext = hubContext;
            _orderRepository = orderRepository;
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId)) return Unauthorized();

                var result = await _orderService.CreateOrderAsync(accountId, dto, HttpContext);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout Error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                var success = await _orderService.HandlePaymentCallback(response);

                if (success) return Ok(new { message = "Thanh toán thành công", data = response });
                return BadRequest(new { message = "Thanh toán thất bại", data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback Error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId)) return Unauthorized();

                var result = await _orderService.GetOrderHistoryAsync(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get History Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out int accountId)) return Unauthorized();

                var result = await _orderService.GetOrderDetailAsync(accountId, id);

                if (result == null) return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get Order Detail Error");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] byte newStatus)
        {
            try
            {
                if (newStatus == OrderStatusConstants.Shipping ||
                    newStatus == OrderStatusConstants.Completed)
                {
                    return BadRequest(new
                    {
                        message = "Admin không được cập nhật thủ công trạng thái 'Đang giao' hoặc 'Hoàn thành'. Trạng thái này sẽ được cập nhật tự động bởi GHN."
                    });
                }

                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null) return NotFound();

                if (order.OrderStatus == OrderStatusConstants.Cancelled)
                {
                    return BadRequest(new { message = "Không thể cập nhật đơn hàng đã bị hủy." });
                }

                await _orderRepository.UpdateOrderStatusAsync(id, newStatus);

                string statusMessage = newStatus switch
                {
                    OrderStatusConstants.Pending => "Đơn hàng đang chờ xử lý.",
                    OrderStatusConstants.Processing => "Đơn hàng đang được đóng gói.",
                    OrderStatusConstants.Confirmed => "Đơn hàng đã được xác nhận.",
                    OrderStatusConstants.Cancelled => "Đơn hàng đã bị hủy bởi Admin.",
                    _ => "Trạng thái đơn hàng đã thay đổi."
                };

                await _hubContext.Clients.Group($"User_{order.UserId}").SendAsync("ReceiveOrderStatus", new
                {
                    OrderId = order.OrderId,
                    NewStatus = newStatus,
                    Message = statusMessage,
                    Timestamp = DateTime.Now
                });

                await _hubContext.Clients.Group("Admins").SendAsync("AdminOrderUpdate", new
                {
                    OrderId = order.OrderId,
                    NewStatus = newStatus,
                    Message = $"Admin vừa cập nhật đơn #{id} sang trạng thái {newStatus}"
                });

                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Status Error");
                return StatusCode(500);
            }
        }
    }
}