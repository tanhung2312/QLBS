using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLBS.Services.Interfaces;
using QLBS.Dtos;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);

                if (result == "Đăng ký tài khoản thành công.")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RefreshTokenAsync(tokenRequestDto);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }
    }
}

