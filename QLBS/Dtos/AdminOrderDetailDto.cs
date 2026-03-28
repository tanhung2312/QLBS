using QLBS.Dtos.Order;

namespace QLBS.Dtos
{
    public class AdminOrderDetailDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? GhnOrderCode { get; set; }
        public string? OrderStatusName { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
