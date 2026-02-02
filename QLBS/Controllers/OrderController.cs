using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Order;
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

        public OrderController(IOrderService orderService, IVnPayService vnPayService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
            _logger = logger;
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
    }
}