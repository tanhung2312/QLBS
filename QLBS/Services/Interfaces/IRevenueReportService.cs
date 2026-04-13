using QLBS.Dtos;

namespace QLBS.Services.Interfaces
{
    public interface IRevenueReportService
    {
        Task<IEnumerable<RevenueReportResponseDto>> GetAllAsync();
        Task<RevenueReportResponseDto?> GetByIdAsync(int id);
        Task<RevenueSummaryDto> GetSummaryAsync(DateTime from, DateTime to);
        Task<RevenueSummaryDto> GetMonthSummaryAsync(int month, int year);
        Task<RevenueReportResponseDto> CreateAsync(RevenueReportCreateDto dto);
        Task<RevenueReportResponseDto?> UpdateAsync(int id, RevenueReportCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<RevenueReportResponseDto> GenerateDailyReportAsync(DateTime date);
    }

    public interface IReportDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardAsync();
    }
}