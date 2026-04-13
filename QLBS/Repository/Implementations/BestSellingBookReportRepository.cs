using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class BestSellingBookReportRepository : IBestSellingBookReportRepository
    {
        private readonly QLBSDbContext _context;

        public BestSellingBookReportRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BestSellingBookReport>> GetAllAsync()
            => await _context.BestSellingBookReports
                .Include(r => r.Book)
                .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
                .ToListAsync();

        public async Task<BestSellingBookReport?> GetByIdAsync(int id)
            => await _context.BestSellingBookReports
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.BestSellingReportId == id);

        public async Task<IEnumerable<BestSellingBookReport>> GetByMonthYearAsync(int month, int year)
            => await _context.BestSellingBookReports
                .Include(r => r.Book)
                .Where(r => r.Month == month && r.Year == year)
                .OrderByDescending(r => r.QuantitySold)
                .ToListAsync();

        public async Task<IEnumerable<BestSellingBookReport>> GetTopAsync(int month, int year, int top = 10)
            => await _context.BestSellingBookReports
                .Include(r => r.Book)
                .Where(r => r.Month == month && r.Year == year)
                .OrderByDescending(r => r.QuantitySold)
                .Take(top)
                .ToListAsync();

        public async Task<IEnumerable<BestSellingBookReport>> GetByBookIdAsync(int bookId)
            => await _context.BestSellingBookReports
                .Include(r => r.Book)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
                .ToListAsync();

        public async Task<BestSellingBookReport> CreateAsync(BestSellingBookReport report)
        {
            _context.BestSellingBookReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<BestSellingBookReport?> UpdateAsync(int id, BestSellingBookReport report)
        {
            var existing = await _context.BestSellingBookReports.FindAsync(id);
            if (existing is null) return null;

            existing.Month = report.Month;
            existing.Year = report.Year;
            existing.BookId = report.BookId;
            existing.QuantitySold = report.QuantitySold;
            existing.Revenue = report.Revenue;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.BestSellingBookReports.FindAsync(id);
            if (report is null) return false;

            _context.BestSellingBookReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int bookId, int month, int year)
            => await _context.BestSellingBookReports
                .AnyAsync(r => r.BookId == bookId && r.Month == month && r.Year == year);
    }
}
