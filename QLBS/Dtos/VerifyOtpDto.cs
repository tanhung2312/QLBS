using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        [StringLength(6, ErrorMessage = "Mã OTP phải có đúng 6 ký tự", MinimumLength = 6)]
        public string OtpCode { get; set; }
    }
}
