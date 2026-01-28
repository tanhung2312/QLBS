using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("RevenueReport")]
    public class RevenueReport
    {
        public int RevenueReportId { get; set; }
        public DateTime ReportDate { get; set; }

        public int OrderCount { get; set; }
        public int SoldBookCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
