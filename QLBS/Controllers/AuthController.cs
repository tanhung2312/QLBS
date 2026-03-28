using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLBS.Services.Interfaces;
using QLBS.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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

        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth",
                new { returnUrl }, Request.Scheme);

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext
                .AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return Redirect($"https://localhost:7241/login?error=Google+đăng+nhập+thất+bại");

            var claims = authenticateResult.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatarUrl = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

            if (string.IsNullOrEmpty(email))
                return Redirect($"https://localhost:7241/login?error=Không+lấy+được+email");

            try
            {
                var result = await _authService.GoogleLoginAsync(email, fullName, avatarUrl);
                if (result == null)
                    return Redirect($"https://localhost:7241/login?error=Không+thể+xử+lý+đăng+nhập");

                // Redirect về FE kèm token trên query string
                return Redirect(
                    $"https://localhost:7241/login" +
                    $"?accessToken={Uri.EscapeDataString(result.AccessToken)}" +
                    $"&refreshToken={Uri.EscapeDataString(result.RefreshToken)}" +
                    $"&userId={result.UserId}" +
                    $"&email={Uri.EscapeDataString(result.Email)}" +
                    $"&fullName={Uri.EscapeDataString(result.FullName ?? "")}" +
                    $"&role={Uri.EscapeDataString(result.Role ?? "")}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Redirect($"https://localhost:7241/login?error={Uri.EscapeDataString(ex.Message)}");
            }
            catch (Exception ex)
            {
                return Redirect($"https://localhost:7241/login?error={Uri.EscapeDataString(ex.Message)}");
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);

                if (result.StartsWith("Không tìm thấy"))
                {
                    return NotFound(new { message = result });
                }

                return Ok(new { message = "OTP đã được gửi đến email của bạn" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isValid = await _authService.VerifyOtpAsync(verifyOtpDto);

                if (isValid)
                {
                    return Ok(new { message = "OTP is valid" });
                }
                else
                {
                    return BadRequest(new { message = "Invalid or expired OTP" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server nội bộ", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResetPasswordAsync(resetPasswordDto);

                if (result == "Đặt lại mật khẩu thành công.")
                {
                    return Ok(new { message = "Password reset successfully" });
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
    }
}
