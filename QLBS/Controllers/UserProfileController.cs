using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos;
using QLBS.Services.Interfaces;
using System.Security.Claims;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPut("user-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
                {
                    return Unauthorized(new { message = "Không thể xác thực tài khoản" });
                }

                var updatedProfile = await _userProfileService.UpdateProfileAsync(accountId, updateProfileDto);

                if (updatedProfile == null)
                {
                    return NotFound(new { message = "Không tìm thấy hồ sơ người dùng hoặc cập nhật thất bại" });
                }

                return Ok(new { message = "Cập nhật hồ sơ thành công", profile = updatedProfile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
                {
                    return Unauthorized(new { message = "Không thể xác thực tài khoản" });
                }

                var userProfile = await _userProfileService.GetUserProfileAsync(accountId);

                if (userProfile == null)
                {
                    return NotFound(new { message = "Không tìm thấy hồ sơ người dùng" });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }
    }
}
