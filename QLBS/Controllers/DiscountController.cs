using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Discount;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")] 
        public async Task<IActionResult> GetAll()
        {
            var result = await _discountService.GetAllDiscountsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _discountService.GetDiscountByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy mã giảm giá" });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateDiscountCodeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest(new { message = "Ngày kết thúc phải lớn hơn ngày bắt đầu" });
            }

            var result = await _discountService.CreateDiscountAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Tạo thất bại. Mã có thể đã tồn tại hoặc lỗi dữ liệu." });

            return StatusCode(201, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _discountService.DeleteDiscountAsync(id);
            if (!result)
                return BadRequest(new { message = "Xóa thất bại. Mã không tồn tại hoặc đã được sử dụng trong đơn hàng." });

            return Ok(new { message = "Xóa thành công" });
        }
    }
}