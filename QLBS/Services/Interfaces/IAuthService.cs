using QLBS.Dtos;

namespace QLBS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
    }
}
