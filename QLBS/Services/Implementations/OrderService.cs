using Microsoft.Extensions.Options;
using QLBS.Constants;
using QLBS.Dtos.Order;
using QLBS.Helpers;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IGhnService _ghnService;
        private readonly IVnPayService _vnPayService;
        private readonly GhnSettings _ghnSettings;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IUserProfileRepository userProfileRepository,
            IDiscountRepository discountRepository,
            IGhnService ghnService,
            IVnPayService vnPayService,
            IOptions<GhnSettings> ghnSettings,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userProfileRepository = userProfileRepository;
            _discountRepository = discountRepository;
            _ghnService = ghnService;
            _vnPayService = vnPayService;
            _ghnSettings = ghnSettings.Value;
            _logger = logger;
        }

        public async Task<OrderResultDto?> CreateOrderAsync(int accountId, CreateOrderDto dto, HttpContext context)
        {
            if (dto.PaymentMethodId != PaymentMethodConstants.COD &&
                dto.PaymentMethodId != PaymentMethodConstants.VNPay)
            {
                throw new ArgumentException("Phương thức thanh toán không hợp lệ.");
            }

            var profile = await _userProfileRepository.GetByAccountIdAsync(accountId);
            if (profile == null) throw new Exception("Không tìm thấy thông tin người dùng.");

            var cartItems = await _cartRepository.GetCartByUserIdAsync(profile.UserId);
            if (!cartItems.Any()) throw new Exception("Giỏ hàng trống.");

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Book.Quantity)
                    throw new Exception($"Sách '{item.Book.BookTitle}' không đủ hàng (Còn: {item.Book.Quantity}).");
            }

            int totalWeight = cartItems.Sum(x => x.Quantity) * _ghnSettings.DefaultWeight;
            decimal shippingFee = 0;
            try
            {
                shippingFee = await _ghnService.CalculateShippingFeeAsync(dto.ToDistrictId, dto.ToWardCode, totalWeight);
            }
            catch
            {
                throw new Exception("Không thể tính phí vận chuyển. Vui lòng kiểm tra lại địa chỉ.");
            }

            decimal itemTotal = cartItems.Sum(x => x.Quantity * x.Book.Price);
            decimal finalTotal = itemTotal + shippingFee;
            int? discountId = null;

            if (!string.IsNullOrEmpty(dto.DiscountCode))
            {
                var discount = await _discountRepository.GetByCodeAsync(dto.DiscountCode);

                if (discount == null || !discount.IsActive)
                    throw new ArgumentException("Mã giảm giá không tồn tại hoặc đã hết hạn.");

                if (discount.StartDate > DateTime.Now || discount.EndDate < DateTime.Now)
                    throw new ArgumentException("Mã giảm giá chưa đến đợt hoặc đã hết hạn.");

                if (discount.Quantity <= 0)
                    throw new ArgumentException("Mã giảm giá đã hết lượt sử dụng.");

                if (discount.MinOrderAmount.HasValue && itemTotal < discount.MinOrderAmount.Value)
                    throw new ArgumentException($"Đơn hàng chưa đủ điều kiện áp dụng mã (Tối thiểu {discount.MinOrderAmount:N0}đ).");

                decimal discountAmount = discount.DiscountType == 0
                    ? discount.DiscountValue
                    : (itemTotal * discount.DiscountValue / 100);

                if (discount.MaxDiscountAmount.HasValue && discountAmount > discount.MaxDiscountAmount.Value)
                    discountAmount = discount.MaxDiscountAmount.Value;

                finalTotal -= discountAmount;
                discountId = discount.DiscountCodeId;
            }

            if (finalTotal < 0) finalTotal = 0;


            var order = new OrderTable
            {
                UserId = profile.UserId,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                ShippingAddress = dto.ShippingAddress,
                OrderDate = DateTime.Now,
                TotalAmount = finalTotal,
                TotalQuantity = cartItems.Sum(x => x.Quantity),
                OrderStatus = OrderStatusConstants.Pending,
                DiscountCodeId = discountId,
                ShippingFee = shippingFee,
                ProvinceID = dto.ToProvinceId,
                DistrictID = dto.ToDistrictId,
                WardCode = dto.ToWardCode,
                GhnOrderCode = null
            };

            var details = cartItems.Select(c => new OrderDetail
            {
                BookId = c.BookId,
                Quantity = c.Quantity,
                UnitPrice = c.Book.Price
            }).ToList();

            var createdOrder = await _orderRepository.CreateOrderAsync(order, details, profile.UserId, dto.PaymentMethodId);


            string paymentUrl = "";
            string message = "Đặt hàng thành công.";

            try
            {
                if (dto.PaymentMethodId == PaymentMethodConstants.VNPay)
                {
                    paymentUrl = _vnPayService.CreatePaymentUrl(context, createdOrder);
                }
                else if (dto.PaymentMethodId == PaymentMethodConstants.COD)
                {
                    var fullOrder = await _orderRepository.GetByIdAsync(createdOrder.OrderId);

                    if (fullOrder != null)
                    {
                        var ghnCode = await _ghnService.CreateShippingOrderAsync(
                            fullOrder,
                            fullOrder.OrderDetails.ToList(),
                            dto.ToDistrictId,
                            dto.ToWardCode,
                            totalWeight,
                            dto.PaymentMethodId
                        );

                        if (string.IsNullOrEmpty(ghnCode))
                        {
                            throw new Exception("Lỗi khi tạo vận đơn GHN.");
                        }

                        await _orderRepository.UpdateOrderGhnCodeAsync(createdOrder.OrderId, ghnCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi integration sau khi tạo đơn. OrderId: {OrderId}", createdOrder.OrderId);

                await _orderRepository.CancelOrderAndRestoreStockAsync(createdOrder.OrderId);

                throw new Exception($"Đặt hàng thất bại do lỗi hệ thống vận chuyển: {ex.Message}");
            }

            return new OrderResultDto
            {
                OrderId = createdOrder.OrderId,
                TotalAmount = createdOrder.TotalAmount,
                PaymentUrl = paymentUrl,
                Message = message
            };
        }

        public async Task<bool> HandlePaymentCallback(PaymentResponseModel model)
        {
            if (!model.Success) return false;
            int orderId = int.Parse(model.OrderId);

            if (model.VnPayResponseCode == "00") 
            {
                await _orderRepository.UpdatePaymentStatusAsync(orderId, PaymentStatusConstants.Success, model.TransactionId);
                await _orderRepository.UpdateOrderStatusAsync(orderId, OrderStatusConstants.Confirmed);

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order != null && string.IsNullOrEmpty(order.GhnOrderCode))
                {
                    int totalWeight = order.OrderDetails.Sum(d => d.Quantity) * _ghnSettings.DefaultWeight;

                    var paymentMethodId = order.Payments.FirstOrDefault()?.PaymentMethodId ?? PaymentMethodConstants.VNPay;

                    var ghnCode = await _ghnService.CreateShippingOrderAsync(
                        order,
                        order.OrderDetails.ToList(),
                        order.DistrictID ?? 0,
                        order.WardCode ?? "",
                        totalWeight,
                        paymentMethodId
                    );
                }
                return true;
            }
            else
            {
                await _orderRepository.UpdatePaymentStatusAsync(orderId, PaymentStatusConstants.Failed, model.TransactionId);
                return false;
            }
        }
    }
}