using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class InventoryReportRepository : IInventoryReportRepository
    {
        private readonly QLBSDbContext _context;

        public InventoryReportRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryReport>> GetAllAsync()
            => await _context.InventoryReports
                .Include(r => r.Book)
                .OrderByDescending(r => r.UpdateDate)
                .ToListAsync();

        public async Task<InventoryReport?> GetByIdAsync(int id)
            => await _context.InventoryReports
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.InventoryReportId == id);

        public async Task<IEnumerable<InventoryReport>> GetLatestAsync()
            => await _context.InventoryReports
                .Include(r => r.Book)
                .GroupBy(r => r.BookId)
                .Select(g => g.OrderByDescending(r => r.UpdateDate).First())
                .OrderBy(r => r.StockQuantity)
                .ToListAsync();

        public async Task<IEnumerable<InventoryReport>> GetLowStockAsync(int threshold = 10)
            => await _context.InventoryReports
                .Include(r => r.Book)
                .GroupBy(r => r.BookId)
                .Select(g => g.OrderByDescending(r => r.UpdateDate).First())
                .Where(r => r.StockQuantity > 0 && r.StockQuantity <= threshold)
                .OrderBy(r => r.StockQuantity)
                .ToListAsync();

        public async Task<IEnumerable<InventoryReport>> GetOutOfStockAsync()
            => await _context.InventoryReports
                .Include(r => r.Book)
                .GroupBy(r => r.BookId)
                .Select(g => g.OrderByDescending(r => r.UpdateDate).First())
                .Where(r => r.StockQuantity == 0)
                .ToListAsync();

        public async Task<InventoryReport?> GetLatestByBookIdAsync(int bookId)
            => await _context.InventoryReports
                .Include(r => r.Book)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.UpdateDate)
                .FirstOrDefaultAsync();

        public async Task<InventoryReport> CreateAsync(InventoryReport report)
        {
            _context.InventoryReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<InventoryReport?> UpdateAsync(int id, InventoryReport report)
        {
            var existing = await _context.InventoryReports.FindAsync(id);
            if (existing is null) return null;

            existing.UpdateDate = report.UpdateDate;
            existing.BookId = report.BookId;
            existing.StockQuantity = report.StockQuantity;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.InventoryReports.FindAsync(id);
            if (report is null) return false;

            _context.InventoryReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
