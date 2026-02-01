using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using QLBS.Services.Interfaces;
using QLBS.Repository.Interfaces;
using QLBS.Dtos;
using QLBS.Models;
using QLBS.Constants;

namespace QLBS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingAccount = await _accountRepository.GetAccountByEmailAsync(registerDto.Email);
                if (existingAccount != null)
                {
                    return "Email đã được sử dụng bởi tài khoản khác.";
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                var customerRoleId = await _accountRepository.GetRoleIdByNameAsync(QLBSRoles.Customer);
                if (customerRoleId == null)
                {
                    return "Lỗi hệ thống: Không tìm thấy role Customer.";
                }

                var newAccount = new Account
                {
                    Email = registerDto.Email,
                    Password = hashedPassword,
                    RoleId = customerRoleId.Value,
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var newProfile = new UserProfile
                {
                    FullName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber,
                    IsDeleted = false
                };

                var success = await _accountRepository.CreateAccountWithProfileAsync(newAccount, newProfile);

                if (success)
                {
                    return "Đăng ký tài khoản thành công.";
                }
                else
                {
                    return "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi hệ thống: {ex.Message}";
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var account = await _accountRepository.GetAccountByEmailAsync(loginDto.Email);
                if (account == null || !account.IsActive)
                {
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
                {
                    return null;
                }

                var accessToken = GenerateAccessToken(account);
                var refreshToken = GenerateRefreshToken();

                account.RefreshToken = refreshToken;
                account.LastLogin = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;

                await _accountRepository.UpdateAccountAsync(account);

                return new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Email = account.Email,
                    FullName = account.UserProfile?.FullName ?? account.Email
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            try
            {
                var account = await _accountRepository.GetAccountByRefreshTokenAsync(tokenRequestDto.RefreshToken);

                if (account == null || !account.IsActive)
                {
                    return null;
                }

                var newAccessToken = GenerateAccessToken(account);
                var newRefreshToken = GenerateRefreshToken();

                account.RefreshToken = newRefreshToken;
                account.UpdatedAt = DateTime.UtcNow;

                await _accountRepository.UpdateAccountAsync(account);

                return new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Email = account.Email,
                    FullName = account.UserProfile?.FullName ?? account.Email
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateAccessToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "your-secret-key-minimum-32-characters-long-for-security");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role?.RoleName ?? QLBSRoles.Customer)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _configuration["JwtSettings:Issuer"] ?? "QLBS_API",
                Audience = _configuration["JwtSettings:Audience"] ?? "QLBS_Client",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[32];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            if (account == null)
            {
                return "Không tìm thấy tài khoản với email này";
            }

            var otp = new Random().Next(100000, 999999).ToString();

            account.OTP = otp;
            account.OtpExpires = DateTime.UtcNow.AddMinutes(15);

            var success = await _accountRepository.UpdateAccountOtpAsync(account);
            if (!success)
            {
                return "Không thể tạo mã OTP. Vui lòng thử lại";
            }
            return otp;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(verifyOtpDto.Email);
            if (account == null)
            {
                return false;
            }

            if (account.OTP == verifyOtpDto.OtpCode && account.OtpExpires > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(resetPasswordDto.Email);
            if (account == null)
            {
                return "Không tìm thấy tài khoản với email này";
            }

            if (account.OTP != resetPasswordDto.OtpCode || account.OtpExpires <= DateTime.UtcNow)
            {
                return "Mã OTP không hợp lệ hoặc đã hết hạn";
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);

            account.Password = hashedPassword;
            account.OTP = null;
            account.OtpExpires = null;

            var success = await _accountRepository.UpdateAccountAsync(account);
            if (!success)
            {
                return "Không thể đặt lại mật khẩu. Vui lòng thử lại";
            }

            return "Đặt lại mật khẩu thành công";
        }
    }
}

