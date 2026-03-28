using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderTable> CreateOrderAsync(OrderTable order, List<OrderDetail> details, int userId, int paymentMethodId);
        Task<OrderTable?> GetByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, byte status);
        Task UpdateOrderGhnCodeAsync(int orderId, string ghnCode);
        Task UpdatePaymentStatusAsync(int orderId, byte paymentStatus, string transactionId);
        Task CancelOrderAndRestoreStockAsync(int orderId);
        Task<IEnumerable<OrderTable>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderTable>> GetAllOrdersAsync();
    }
}