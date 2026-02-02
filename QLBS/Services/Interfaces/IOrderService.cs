using QLBS.Dtos.Order;
using QLBS.Models;

namespace QLBS.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResultDto?> CreateOrderAsync(int accountId, CreateOrderDto dto, HttpContext context);
        Task<bool> HandlePaymentCallback(PaymentResponseModel model);
        Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(int accountId);
        Task<OrderDetailResponseDto?> GetOrderDetailAsync(int accountId, int orderId);
    }
}