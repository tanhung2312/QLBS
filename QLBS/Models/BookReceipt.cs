using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("BookReceipt")]
    public class BookReceipt
    {
        [Key]
        public int ReceiptId { get; set; }
        public int SupplierId { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public byte Status { get; set; }

        public int? ConfirmedById { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public string? Note { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("ConfirmedById")]
        public virtual UserProfile ConfirmedBy { get; set; }

        public virtual ICollection<BookReceiptDetail> BookReceiptDetails { get; set; }
    }
}
