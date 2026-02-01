using System;
using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos
{
    public class UpdateProfileDto
    {
        [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [RegularExpression(@"^\+?[0-9]{7,20}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "URL ảnh không được vượt quá 500 ký tự")]
        public string? AvatarUrl { get; set; }
    }
}
