namespace QLBS.Constants
{
    public static class OrderStatusConstants
    {
        public const byte Pending = 0;      // Chờ xử lý / Chờ thanh toán
        public const byte Processing = 1;   // Đang đóng gói
        public const byte Confirmed = 2;    // Đã xác nhận / Đã thanh toán
        public const byte Shipping = 3;     // Đang giao hàng
        public const byte Completed = 4;    // Giao thành công
        public const byte Cancelled = 5;    // Đã hủy
    }
}