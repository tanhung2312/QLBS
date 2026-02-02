using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Cart;
using QLBS.Services.Interfaces;
using System.Security.Claims;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out int accountId))
            {
                return 0;
            }
            return accountId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                if (accountId == 0) return Unauthorized(new { message = "Token không hợp lệ" });

                var cart = await _cartService.GetUserCartAsync(accountId);

                if (cart == null) return NotFound(new { message = "Không tìm thấy thông tin người dùng" });

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy giỏ hàng");
                return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var accountId = GetCurrentAccountId();
                if (accountId == 0) return Unauthorized();

                var result = await _cartService.AddToCartAsync(accountId, dto);

                if (result != "Success")
                {
                    return BadRequest(new { message = result });
                }

                return Ok(new { message = "Đã thêm vào giỏ hàng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng. AccountId: {AccountId}, BookId: {BookId}", GetCurrentAccountId(), dto.BookId);
                return StatusCode(500, new { message = "Lỗi hệ thống khi thêm giỏ hàng." });
            }
        }

        [HttpPut("update/{bookId}")]
        public async Task<IActionResult> UpdateQuantity(int bookId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                if (accountId == 0) return Unauthorized();

                var success = await _cartService.UpdateQuantityAsync(accountId, bookId, dto.Quantity);

                if (!success)
                    return BadRequest(new { message = "Cập nhật thất bại (Sách không có trong giỏ hoặc vượt quá tồn kho)" });

                return Ok(new { message = "Cập nhật số lượng thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi update số lượng. AccountId: {Id}", GetCurrentAccountId());
                return StatusCode(500, new { message = "Lỗi hệ thống." });
            }
        }

        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                if (accountId == 0) return Unauthorized();

                var success = await _cartService.RemoveFromCartAsync(accountId, bookId);

                if (!success) return NotFound(new { message = "Sách không có trong giỏ hàng" });

                return Ok(new { message = "Đã xóa sách khỏi giỏ hàng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa item giỏ hàng");
                return StatusCode(500, new { message = "Lỗi hệ thống." });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                if (accountId == 0) return Unauthorized();

                await _cartService.ClearCartAsync(accountId);
                return Ok(new { message = "Đã làm trống giỏ hàng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi clear giỏ hàng");
                return StatusCode(500, new { message = "Lỗi hệ thống." });
            }
        }
    }
}