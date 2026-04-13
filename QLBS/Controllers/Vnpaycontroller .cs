using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Models;
using QLBS.Services.Interfaces;
using System.Security.Claims;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<VnPayController> _logger;

        public VnPayController(
            IVnPayService vnPayService,
            ILogger<VnPayController> logger)
        {
            _vnPayService = vnPayService;
            _logger = logger;
        }

        [HttpPost("create-payment")]
        [Authorize]
        public IActionResult CreatePayment([FromBody] CreateVnPayRequest request)
        {
            if (request.OrderId <= 0 || request.Amount <= 0)
                return BadRequest(new { message = "OrderId hoặc Amount không hợp lệ." });

            var order = new OrderTable
            {
                OrderId = request.OrderId,
                TotalAmount = request.Amount
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, order);

            _logger.LogInformation(
                "[VnPay] CreatePayment: orderId={OrderId} amount={Amount}",
                request.OrderId, request.Amount);

            return Ok(new { paymentUrl });
        }


        [HttpGet("payment-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback()
        {
            const string frontendUrl = "https://localhost:7241";

            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
            {
                _logger.LogWarning("[VnPay] Callback: chữ ký không hợp lệ.");
                return Redirect($"{frontendUrl}/payment-result?success=false&reason=invalid_signature");
            }

            _logger.LogInformation("[VnPay] Callback: orderId={OrderId} code={Code} txnId={TxnId}",
                response.OrderId, response.VnPayResponseCode, response.TransactionId);

            var handled = await _vnPayService.HandlePaymentCallbackAsync(response);

            if (!handled)
            {
                _logger.LogError("[VnPay] HandlePaymentCallback thất bại: orderId={OrderId}", response.OrderId);
                return Redirect($"{frontendUrl}/payment-result?success=false&reason=update_failed&orderId={response.OrderId}");
            }

            if (response.VnPayResponseCode == "00")
                return Redirect($"{frontendUrl}/payment-result?success=true&orderId={response.OrderId}");

            return Redirect($"{frontendUrl}/payment-result?success=false&reason=vnpay_error&code={response.VnPayResponseCode}&orderId={response.OrderId}");
        }


        [HttpPost("ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> Ipn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
            {
                _logger.LogWarning("[VnPay] IPN: chữ ký không hợp lệ.");
                return Ok(new { RspCode = "97", Message = "Invalid signature" });
            }

            if (response.VnPayResponseCode != "00")
            {
                _logger.LogWarning("[VnPay] IPN: thanh toán thất bại code={Code}", response.VnPayResponseCode);
                return Ok(new { RspCode = response.VnPayResponseCode, Message = "Payment failed" });
            }

            var handled = await _vnPayService.HandlePaymentCallbackAsync(response);

            if (!handled)
            {
                _logger.LogError("[VnPay] IPN: HandlePaymentCallback thất bại orderId={OrderId}", response.OrderId);
                return Ok(new { RspCode = "99", Message = "Update failed" });
            }

            _logger.LogInformation("[VnPay] IPN OK: orderId={OrderId} txnId={TxnId}",
                response.OrderId, response.TransactionId);

            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }
    }

    public class CreateVnPayRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}