namespace QLBS.Dtos
{

    public class RevenueReportResponseDto
    {
        public int RevenueReportId { get; set; }
        public DateTime ReportDate { get; set; }
        public int OrderCount { get; set; }
        public int SoldBookCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueReportCreateDto
    {
        public DateTime ReportDate { get; set; }
        public int OrderCount { get; set; }
        public int SoldBookCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueSummaryDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBooksSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenuePerDay { get; set; }
        public List<RevenueReportResponseDto> Details { get; set; } = new();
    }


    public class BestSellingBookReportResponseDto
    {
        public int BestSellingReportId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public string? BookImageUrl { get; set; }
        public string? AuthorName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class BestSellingBookReportCreateDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int BookId { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopSellingBooksDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<BestSellingBookReportResponseDto> Books { get; set; } = new();
    }


    public class InventoryReportResponseDto
    {
        public int InventoryReportId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public string? BookImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string StockStatus { get; set; } = "";
    }

    public class InventoryReportCreateDto
    {
        public int BookId { get; set; }
        public int StockQuantity { get; set; }
    }

    public class InventorySummaryDto
    {
        public DateTime ReportTime { get; set; }
        public int TotalBooks { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int InStockCount { get; set; }
        public List<InventoryReportResponseDto> Details { get; set; } = new();
    }


    public class DashboardSummaryDto
    {
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenueLastMonth { get; set; }
        public decimal RevenueGrowthPercent { get; set; }
        public int TotalOrdersThisMonth { get; set; }
        public int TotalBooksSoldThisMonth { get; set; }
        public int OutOfStockBooks { get; set; }
        public int LowStockBooks { get; set; }
        public List<BestSellingBookReportResponseDto> TopSellingBooks { get; set; } = new();
        public List<RevenueReportResponseDto> RecentRevenueReports { get; set; } = new();
    }
}