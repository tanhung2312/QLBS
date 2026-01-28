using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("InventoryReport")]
    public class InventoryReport
    {
        public int InventoryReportId { get; set; }
        public DateTime UpdateDate { get; set; }

        public int BookId { get; set; }
        public int StockQuantity { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
