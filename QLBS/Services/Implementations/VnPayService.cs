using Microsoft.Extensions.Options;
using QLBS.Helpers;
using QLBS.Models;
using QLBS.Services.Interfaces;
using QLCHBS.Utils;

namespace QLBS.Services.Implementations
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _config;

        public VnPayService(IOptions<VnPaySettings> config)
        {
            _config = config.Value;
        }

        public string CreatePaymentUrl(HttpContext context, OrderTable order)
        {
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config.TmnCode);
            // Số tiền (VNPay yêu cầu nhân 100)
            vnpay.AddRequestData("vnp_Amount", ((long)(order.TotalAmount * 100)).ToString());

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context)); // Cần viết hàm GetIpAddress
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh_toan_don_hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            return vnpay.CreateRequestUrl(_config.BaseUrl, _config.HashSecret);
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config.HashSecret);

            if (!checkSignature)
            {
                return new PaymentResponseModel { Success = false };
            }

            return new PaymentResponseModel
            {
                Success = true,
                OrderId = vnp_orderId,
                TransactionId = vnp_TransactionId,
                OrderInfo = vnp_OrderInfo,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}