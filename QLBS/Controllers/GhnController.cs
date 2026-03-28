using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly IGhnService _ghnService;

        public GhnController(IGhnService ghnService)
        {
            _ghnService = ghnService;
        }

     
        [HttpGet("calculate-fee")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculateFee([FromQuery] int districtId, [FromQuery] string wardCode, [FromQuery] int totalWeight = 200)
        {
            if (districtId <= 0 || string.IsNullOrWhiteSpace(wardCode))
            {
                return BadRequest(new { message = "Vui lòng chọn Quận/Huyện và Phường/Xã hợp lệ để tính phí vận chuyển." });
            }

            try
            {
                decimal fee = await _ghnService.CalculateShippingFeeAsync(districtId, wardCode, totalWeight);

                return Ok(new { shippingFee = fee });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi kết nối với hệ thống Giao Hàng Nhanh.", error = ex.Message });
            }
        }



        [HttpGet("provinces")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _ghnService.GetProvincesAsync();
            return Ok(result);
        }

        [HttpGet("districts/{provinceId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var result = await _ghnService.GetDistrictsAsync(provinceId);
            return Ok(result);
        }

        [HttpGet("wards/{districtId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var result = await _ghnService.GetWardsAsync(districtId);
            return Ok(result);
        }
    }
}
