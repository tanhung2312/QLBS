using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{

    public class RevenueReportRepository : IRevenueReportRepository
    {
        private readonly QLBSDbContext _context;

        public RevenueReportRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RevenueReport>> GetAllAsync()
            => await _context.RevenueReports
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

        public async Task<RevenueReport?> GetByIdAsync(int id)
            => await _context.RevenueReports.FindAsync(id);

        public async Task<IEnumerable<RevenueReport>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await _context.RevenueReports
                .Where(r => r.ReportDate.Date >= from.Date && r.ReportDate.Date <= to.Date)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

        public async Task<IEnumerable<RevenueReport>> GetByMonthAsync(int month, int year)
            => await _context.RevenueReports
                .Where(r => r.ReportDate.Month == month && r.ReportDate.Year == year)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

        public async Task<RevenueReport?> GetByDateAsync(DateTime date)
            => await _context.RevenueReports
                .FirstOrDefaultAsync(r => r.ReportDate.Date == date.Date);

        public async Task<RevenueReport> CreateAsync(RevenueReport report)
        {
            _context.RevenueReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<RevenueReport?> UpdateAsync(int id, RevenueReport report)
        {
            var existing = await _context.RevenueReports.FindAsync(id);
            if (existing is null) return null;

            existing.ReportDate = report.ReportDate;
            existing.OrderCount = report.OrderCount;
            existing.SoldBookCount = report.SoldBookCount;
            existing.TotalRevenue = report.TotalRevenue;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.RevenueReports.FindAsync(id);
            if (report is null) return false;

            _context.RevenueReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(DateTime date)
            => await _context.RevenueReports
                .AnyAsync(r => r.ReportDate.Date == date.Date);
    } 
}