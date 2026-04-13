using Microsoft.EntityFrameworkCore;
using QLBS.Dtos;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IRevenueReportRepository _repo;
        private readonly QLBSDbContext _context;

        public RevenueReportService(IRevenueReportRepository repo, QLBSDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<RevenueReportResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<RevenueReportResponseDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            return r is null ? null : MapToDto(r);
        }

        public async Task<RevenueSummaryDto> GetSummaryAsync(DateTime from, DateTime to)
        {
            var list = (await _repo.GetByDateRangeAsync(from, to)).ToList();
            int days = Math.Max(1, (to.Date - from.Date).Days + 1);
            decimal totalRevenue = list.Sum(r => r.TotalRevenue);
            return new RevenueSummaryDto
            {
                FromDate = from,
                ToDate = to,
                TotalOrders = list.Sum(r => r.OrderCount),
                TotalBooksSold = list.Sum(r => r.SoldBookCount),
                TotalRevenue = totalRevenue,
                AverageRevenuePerDay = totalRevenue / days,
                Details = list.Select(MapToDto).ToList()
            };
        }

        public async Task<RevenueSummaryDto> GetMonthSummaryAsync(int month, int year)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddDays(-1);
            return await GetSummaryAsync(from, to);
        }

        public async Task<RevenueReportResponseDto> CreateAsync(RevenueReportCreateDto dto)
        {
            var report = new RevenueReport
            {
                ReportDate = dto.ReportDate,
                OrderCount = dto.OrderCount,
                SoldBookCount = dto.SoldBookCount,
                TotalRevenue = dto.TotalRevenue
            };
            var created = await _repo.CreateAsync(report);
            return MapToDto(created);
        }

        public async Task<RevenueReportResponseDto?> UpdateAsync(int id, RevenueReportCreateDto dto)
        {
            var report = new RevenueReport
            {
                ReportDate = dto.ReportDate,
                OrderCount = dto.OrderCount,
                SoldBookCount = dto.SoldBookCount,
                TotalRevenue = dto.TotalRevenue
            };
            var updated = await _repo.UpdateAsync(id, report);
            return updated is null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);


        public async Task<RevenueReportResponseDto> GenerateDailyReportAsync(DateTime date)
        {
            var orders = await _context.OrderTables
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDate.Date == date.Date && o.OrderStatus == 3)
                .ToListAsync();

            var report = new RevenueReport
            {
                ReportDate = date.Date,
                OrderCount = orders.Count,
                SoldBookCount = orders.SelectMany(o => o.OrderDetails).Sum(d => d.Quantity),
                TotalRevenue = orders.Sum(o => o.TotalAmount)
            };

            var existing = await _repo.GetByDateAsync(date);
            if (existing is not null)
            {
                report.RevenueReportId = existing.RevenueReportId;
                var updated = await _repo.UpdateAsync(existing.RevenueReportId, report);
                return MapToDto(updated!);
            }

            var created = await _repo.CreateAsync(report);
            return MapToDto(created);
        }

        private static RevenueReportResponseDto MapToDto(RevenueReport r) => new()
        {
            RevenueReportId = r.RevenueReportId,
            ReportDate = r.ReportDate,
            OrderCount = r.OrderCount,
            SoldBookCount = r.SoldBookCount,
            TotalRevenue = r.TotalRevenue
        };
    }



    public class ReportDashboardService : IReportDashboardService
    {
        private readonly IRevenueReportService _revenueService;
        private readonly IBestSellingBookReportService _bestSellingService;
        private readonly IInventoryReportService _inventoryService;

        public ReportDashboardService(
            IRevenueReportService revenueService,
            IBestSellingBookReportService bestSellingService,
            IInventoryReportService inventoryService)
        {
            _revenueService = revenueService;
            _bestSellingService = bestSellingService;
            _inventoryService = inventoryService;
        }

        public async Task<DashboardSummaryDto> GetDashboardAsync()
        {
            var now = DateTime.Now;
            var thisMonth = await _revenueService.GetMonthSummaryAsync(now.Month, now.Year);

            var lastMonthDate = now.AddMonths(-1);
            var lastMonth = await _revenueService.GetMonthSummaryAsync(lastMonthDate.Month, lastMonthDate.Year);

            var revenueGrowth = lastMonth.TotalRevenue == 0 ? 0
                : Math.Round((thisMonth.TotalRevenue - lastMonth.TotalRevenue) / lastMonth.TotalRevenue * 100, 2);

            var topBooks = await _bestSellingService.GetTopAsync(now.Month, now.Year, 5);
            var inventory = await _inventoryService.GetLatestSummaryAsync();

            return new DashboardSummaryDto
            {
                TotalRevenueThisMonth = thisMonth.TotalRevenue,
                TotalRevenueLastMonth = lastMonth.TotalRevenue,
                RevenueGrowthPercent = revenueGrowth,
                TotalOrdersThisMonth = thisMonth.TotalOrders,
                TotalBooksSoldThisMonth = thisMonth.TotalBooksSold,
                OutOfStockBooks = inventory.OutOfStockCount,
                LowStockBooks = inventory.LowStockCount,
                TopSellingBooks = topBooks.Books,
                RecentRevenueReports = thisMonth.Details.Take(7).ToList()
            };
        }
    }
}