using Microsoft.EntityFrameworkCore;
using QLBS.Dtos;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class BestSellingBookReportService : IBestSellingBookReportService
    {
        private readonly IBestSellingBookReportRepository _repo;
        private readonly QLBSDbContext _context;

        public BestSellingBookReportService(
            IBestSellingBookReportRepository repo,
            QLBSDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<BestSellingBookReportResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<BestSellingBookReportResponseDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            return r is null ? null : MapToDto(r);
        }

        public async Task<TopSellingBooksDto> GetTopAsync(int month, int year, int top = 10)
        {
            var list = await _repo.GetTopAsync(month, year, top);
            return new TopSellingBooksDto
            {
                Month = month,
                Year = year,
                Books = list.Select(MapToDto).ToList()
            };
        }

        public async Task<IEnumerable<BestSellingBookReportResponseDto>> GetByBookIdAsync(int bookId)
        {
            var list = await _repo.GetByBookIdAsync(bookId);
            return list.Select(MapToDto);
        }

        public async Task<BestSellingBookReportResponseDto> CreateAsync(BestSellingBookReportCreateDto dto)
        {
            var report = new BestSellingBookReport
            {
                Month = dto.Month,
                Year = dto.Year,
                BookId = dto.BookId,
                QuantitySold = dto.QuantitySold,
                Revenue = dto.Revenue
            };
            var created = await _repo.CreateAsync(report);
            var reloaded = await _repo.GetByIdAsync(created.BestSellingReportId);
            return MapToDto(reloaded!);
        }

        public async Task<BestSellingBookReportResponseDto?> UpdateAsync(int id, BestSellingBookReportCreateDto dto)
        {
            var report = new BestSellingBookReport
            {
                Month = dto.Month,
                Year = dto.Year,
                BookId = dto.BookId,
                QuantitySold = dto.QuantitySold,
                Revenue = dto.Revenue
            };
            var updated = await _repo.UpdateAsync(id, report);
            if (updated is null) return null;
            var reloaded = await _repo.GetByIdAsync(id);
            return MapToDto(reloaded!);
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);


        public async Task<TopSellingBooksDto> GenerateMonthlyReportAsync(int month, int year)
        {
            var stats = await _context.OrderDetails
                .Include(d => d.Order)
                .Include(d => d.Book)
                .Where(d => d.Order.OrderDate.Month == month
                         && d.Order.OrderDate.Year == year
                         && d.Order.OrderStatus == 3)
                .GroupBy(d => d.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    QuantitySold = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Quantity * d.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToListAsync();

            foreach (var stat in stats)
            {
                if (await _repo.ExistsAsync(stat.BookId, month, year))
                {
                    var existing = (await _repo.GetByBookIdAsync(stat.BookId))
                        .FirstOrDefault(r => r.Month == month && r.Year == year);
                    if (existing is not null)
                    {
                        existing.QuantitySold = stat.QuantitySold;
                        existing.Revenue = stat.Revenue;
                        await _repo.UpdateAsync(existing.BestSellingReportId, existing);
                    }
                }
                else
                {
                    await _repo.CreateAsync(new BestSellingBookReport
                    {
                        Month = month,
                        Year = year,
                        BookId = stat.BookId,
                        QuantitySold = stat.QuantitySold,
                        Revenue = stat.Revenue
                    });
                }
            }

            var final = await _repo.GetByMonthYearAsync(month, year);
            return new TopSellingBooksDto
            {
                Month = month,
                Year = year,
                Books = final.Select(MapToDto).ToList()
            };
        }

        private static BestSellingBookReportResponseDto MapToDto(BestSellingBookReport r) => new()
        {
            BestSellingReportId = r.BestSellingReportId,
            Month = r.Month,
            Year = r.Year,
            BookId = r.BookId,
            BookTitle = r.Book?.BookTitle ?? "",
            BookImageUrl = null,
            AuthorName = r.Book?.Author?.AuthorName,
            QuantitySold = r.QuantitySold,
            Revenue = r.Revenue
        };
    }

}
