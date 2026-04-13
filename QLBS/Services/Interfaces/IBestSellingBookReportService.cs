using QLBS.Dtos;

namespace QLBS.Services.Interfaces
{
    public interface IBestSellingBookReportService
    {
        Task<IEnumerable<BestSellingBookReportResponseDto>> GetAllAsync();
        Task<BestSellingBookReportResponseDto?> GetByIdAsync(int id);
        Task<TopSellingBooksDto> GetTopAsync(int month, int year, int top = 10);
        Task<IEnumerable<BestSellingBookReportResponseDto>> GetByBookIdAsync(int bookId);
        Task<BestSellingBookReportResponseDto> CreateAsync(BestSellingBookReportCreateDto dto);
        Task<BestSellingBookReportResponseDto?> UpdateAsync(int id, BestSellingBookReportCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<TopSellingBooksDto> GenerateMonthlyReportAsync(int month, int year);
    }
}
