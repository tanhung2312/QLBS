using QLBS.Dtos;

namespace QLBS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
