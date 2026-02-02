using Microsoft.EntityFrameworkCore;
using QLBS.Constants;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly QLBSDbContext _context;

        public OrderRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<OrderTable> CreateOrderAsync(OrderTable order, List<OrderDetail> details, int userId, int paymentMethodId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.OrderTables.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var detail in details)
                {
                    detail.OrderId = order.OrderId;
                    await _context.OrderDetails.AddAsync(detail);

                    var book = await _context.Books.FindAsync(detail.BookId);
                    if (book != null)
                    {
                        if (book.Quantity < detail.Quantity) throw new Exception($"Sách {book.BookTitle} hết hàng.");
                        book.Quantity -= detail.Quantity;
                        _context.Books.Update(book);
                    }
                }
                await _context.SaveChangesAsync();

                var cartItems = _context.Carts.Where(c => c.UserId == userId);
                _context.Carts.RemoveRange(cartItems);

                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethodId = paymentMethodId,
                    PaidAmount = order.TotalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = PaymentStatusConstants.Pending,
                    TransactionCode = ""
                };
                await _context.Payments.AddAsync(payment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, byte status)
        {
            var order = await _context.OrderTables.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderGhnCodeAsync(int orderId, string ghnCode)
        {
            var order = await _context.OrderTables.FindAsync(orderId);
            if (order != null)
            {
                order.GhnOrderCode = ghnCode;
                order.OrderStatus = OrderStatusConstants.Processing;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePaymentStatusAsync(int orderId, byte status, string? transactionCode)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.PaymentStatus = status;
                payment.TransactionCode = transactionCode;
                payment.PaymentDate = DateTime.Now;
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OrderTable?> GetByIdAsync(int id)
        {
            return await _context.OrderTables
                .Include(o => o.Payments)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                        .ThenInclude(b => b.BookImages)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task CancelOrderAndRestoreStockAsync(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.OrderTables
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null) return;

                order.OrderStatus = OrderStatusConstants.Cancelled;

                foreach (var detail in order.OrderDetails)
                {
                    var book = await _context.Books.FindAsync(detail.BookId);
                    if (book != null)
                    {
                        book.Quantity += detail.Quantity;
                        _context.Books.Update(book);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderTable>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.OrderTables
                .AsNoTracking()
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}