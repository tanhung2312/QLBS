using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("BestSellingBookReport")]
    public class BestSellingBookReport
    {
        [Key]
        public int BestSellingReportId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public int BookId { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
