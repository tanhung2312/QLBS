namespace QLBS.Constants
{
    public static class PaymentMethodConstants
    {
        public const int COD = 1;
        public const int VNPay = 2;
        public static string GetMethodName(int id)
        {
            return id switch
            {
                COD => "Thanh toán khi nhận hàng (COD)",
                VNPay => "Thanh toán qua VNPay",
                _ => "Không xác định"
            };
        }
    }
}