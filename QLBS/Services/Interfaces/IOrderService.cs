using QLBS.Dtos;
using QLBS.Dtos.Order;

namespace QLBS.Services.Interfaces
{
    public interface IOrderService
    {

        Task<OrderResultDto?> CreateOrderAsync(int accountId, CreateOrderDto dto, HttpContext context);
        Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(int accountId);
        Task<OrderDetailResponseDto?> GetOrderDetailAsync(int accountId, int orderId);


        Task<IEnumerable<AdminOrderSummaryDto>> GetAllOrdersForAdminAsync();
        Task<AdminOrderDetailDto?> GetOrderDetailForAdminAsync(int orderId);
    }
}