using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Discount
{
    public class DiscountCodeResponseDto
    {
        public int DiscountCodeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public byte DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDiscountCodeDto
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Mã chỉ được chứa chữ và số")]
        public string Code { get; set; } = string.Empty;
        [Required]
        public byte DiscountType { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn hoặc bằng 0")]
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
    }
}