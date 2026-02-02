using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Order
{
    public class CreateOrderDto
    {
        [Required] public string ReceiverName { get; set; } = string.Empty;
        [Required] public string ReceiverPhone { get; set; } = string.Empty;
        [Required] public string ShippingAddress { get; set; } = string.Empty;

        [Required] public int ToProvinceId { get; set; }
        [Required] public int ToDistrictId { get; set; }
        [Required] public string ToWardCode { get; set; } = string.Empty;

        [Required] public int PaymentMethodId { get; set; }
        public string? DiscountCode { get; set; }
    }

    public class OrderResultDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}