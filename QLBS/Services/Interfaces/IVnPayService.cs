using QLBS.Helpers;
using QLBS.Models;
using Microsoft.Extensions.Options;
using QLCHBS.Utils;

namespace QLBS.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, OrderTable order);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}

public class PaymentResponseModel
{
    public bool Success { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string VnPayResponseCode { get; set; } = string.Empty;
}