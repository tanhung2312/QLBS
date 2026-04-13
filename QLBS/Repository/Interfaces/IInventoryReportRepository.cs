using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IInventoryReportRepository
    {
        Task<IEnumerable<InventoryReport>> GetAllAsync();
        Task<InventoryReport?> GetByIdAsync(int id);
        Task<IEnumerable<InventoryReport>> GetLatestAsync();
        Task<IEnumerable<InventoryReport>> GetLowStockAsync(int threshold = 10);
        Task<IEnumerable<InventoryReport>> GetOutOfStockAsync();
        Task<InventoryReport?> GetLatestByBookIdAsync(int bookId);
        Task<InventoryReport> CreateAsync(InventoryReport report);
        Task<InventoryReport?> UpdateAsync(int id, InventoryReport report);
        Task<bool> DeleteAsync(int id);
    }
}
