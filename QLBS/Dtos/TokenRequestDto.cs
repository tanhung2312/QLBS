using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos
{
    public class TokenRequestDto
    {
        [Required(ErrorMessage = "RefreshToken là bắt buộc")]
        public string RefreshToken { get; set; }
    }
}
