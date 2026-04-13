using Microsoft.EntityFrameworkCore;
using QLBS.Dtos;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class InventoryReportService : IInventoryReportService
    {
        private readonly IInventoryReportRepository _repo;
        private readonly QLBSDbContext _context;

        public InventoryReportService(
            IInventoryReportRepository repo,
            QLBSDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<InventoryReportResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<InventoryReportResponseDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            return r is null ? null : MapToDto(r);
        }

        public async Task<InventorySummaryDto> GetLatestSummaryAsync()
        {
            var latest = (await _repo.GetLatestAsync()).ToList();
            return BuildSummary(latest);
        }

        public async Task<IEnumerable<InventoryReportResponseDto>> GetLowStockAsync(int threshold = 10)
        {
            var list = await _repo.GetLowStockAsync(threshold);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<InventoryReportResponseDto>> GetOutOfStockAsync()
        {
            var list = await _repo.GetOutOfStockAsync();
            return list.Select(MapToDto);
        }

        public async Task<InventoryReportResponseDto> CreateAsync(InventoryReportCreateDto dto)
        {
            var report = new InventoryReport
            {
                UpdateDate = DateTime.Now,
                BookId = dto.BookId,
                StockQuantity = dto.StockQuantity
            };
            var created = await _repo.CreateAsync(report);
            var reloaded = await _repo.GetByIdAsync(created.InventoryReportId);
            return MapToDto(reloaded!);
        }

        public async Task<InventoryReportResponseDto?> UpdateAsync(int id, InventoryReportCreateDto dto)
        {
            var report = new InventoryReport
            {
                UpdateDate = DateTime.Now,
                BookId = dto.BookId,
                StockQuantity = dto.StockQuantity
            };
            var updated = await _repo.UpdateAsync(id, report);
            if (updated is null) return null;
            var reloaded = await _repo.GetByIdAsync(id);
            return MapToDto(reloaded!);
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<InventorySummaryDto> GenerateSnapshotAsync()
        {
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .ToListAsync();

            var now = DateTime.Now;
            var reports = new List<InventoryReport>();

            foreach (var book in books)
            {
                var report = new InventoryReport
                {
                    UpdateDate = now,
                    BookId = book.BookId,
                    StockQuantity = book.Quantity
                };
                var created = await _repo.CreateAsync(report);
                created.Book = book;
                reports.Add(created);
            }

            return BuildSummary(reports);
        }

        private static InventorySummaryDto BuildSummary(List<InventoryReport> list)
        {
            const int lowThreshold = 10;
            return new InventorySummaryDto
            {
                ReportTime = DateTime.Now,
                TotalBooks = list.Count,
                OutOfStockCount = list.Count(r => r.StockQuantity == 0),
                LowStockCount = list.Count(r => r.StockQuantity > 0 && r.StockQuantity <= lowThreshold),
                InStockCount = list.Count(r => r.StockQuantity > lowThreshold),
                Details = list.Select(MapToDto).ToList()
            };
        }

        private static InventoryReportResponseDto MapToDto(InventoryReport r) => new()
        {
            InventoryReportId = r.InventoryReportId,
            UpdateDate = r.UpdateDate,
            BookId = r.BookId,
            BookTitle = r.Book?.BookTitle ?? "",
            BookImageUrl = null,
            StockQuantity = r.StockQuantity,
            StockStatus = r.StockQuantity == 0 ? "Hết hàng"
                        : r.StockQuantity <= 10 ? "Sắp hết"
                        : "Còn hàng"
        };
    }

}
