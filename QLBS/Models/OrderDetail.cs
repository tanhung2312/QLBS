using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderTable Order { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
