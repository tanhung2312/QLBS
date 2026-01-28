using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("BookReceiptDetail")]
    public class BookReceiptDetail
    {
        public int ReceiptId { get; set; }
        public int RowNumber { get; set; }
        public int BookId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }

        [ForeignKey("ReceiptId")]
        public virtual BookReceipt BookReceipt { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
