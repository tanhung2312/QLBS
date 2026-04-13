using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IBestSellingBookReportRepository
    {
        Task<IEnumerable<BestSellingBookReport>> GetAllAsync();
        Task<BestSellingBookReport?> GetByIdAsync(int id);
        Task<IEnumerable<BestSellingBookReport>> GetByMonthYearAsync(int month, int year);
        Task<IEnumerable<BestSellingBookReport>> GetTopAsync(int month, int year, int top = 10);
        Task<IEnumerable<BestSellingBookReport>> GetByBookIdAsync(int bookId);
        Task<BestSellingBookReport> CreateAsync(BestSellingBookReport report);
        Task<BestSellingBookReport?> UpdateAsync(int id, BestSellingBookReport report);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int bookId, int month, int year);
    }
}
