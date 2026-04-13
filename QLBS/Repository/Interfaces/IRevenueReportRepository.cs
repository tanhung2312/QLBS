using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IRevenueReportRepository
    {
        Task<IEnumerable<RevenueReport>> GetAllAsync();
        Task<RevenueReport?> GetByIdAsync(int id);
        Task<IEnumerable<RevenueReport>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<RevenueReport>> GetByMonthAsync(int month, int year);
        Task<RevenueReport?> GetByDateAsync(DateTime date);
        Task<RevenueReport> CreateAsync(RevenueReport report);
        Task<RevenueReport?> UpdateAsync(int id, RevenueReport report);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(DateTime date);
    }
}
