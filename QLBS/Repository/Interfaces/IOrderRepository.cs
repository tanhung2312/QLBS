using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderTable> CreateOrderAsync(OrderTable order, List<OrderDetail> details, int userId, int paymentMethodId);
        Task<OrderTable?> GetByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, byte status);
        Task UpdatePaymentStatusAsync(int orderId, byte status, string? transactionCode);
        Task UpdateOrderGhnCodeAsync(int orderId, string ghnCode);
        Task CancelOrderAndRestoreStockAsync(int orderId);
        Task<IEnumerable<OrderTable>> GetOrdersByUserIdAsync(int userId);
    }
}