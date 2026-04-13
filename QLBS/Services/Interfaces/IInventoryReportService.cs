using QLBS.Dtos;

namespace QLBS.Services.Interfaces
{
    public interface IInventoryReportService
    {
        Task<IEnumerable<InventoryReportResponseDto>> GetAllAsync();
        Task<InventoryReportResponseDto?> GetByIdAsync(int id);
        Task<InventorySummaryDto> GetLatestSummaryAsync();
        Task<IEnumerable<InventoryReportResponseDto>> GetLowStockAsync(int threshold = 10);
        Task<IEnumerable<InventoryReportResponseDto>> GetOutOfStockAsync();
        Task<InventoryReportResponseDto> CreateAsync(InventoryReportCreateDto dto);
        Task<InventoryReportResponseDto?> UpdateAsync(int id, InventoryReportCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<InventorySummaryDto> GenerateSnapshotAsync();
    }
}
