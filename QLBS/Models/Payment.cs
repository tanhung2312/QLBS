using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Payment")]
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int PaymentMethodId { get; set; }

        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public byte PaymentStatus { get; set; }

        public string? TransactionCode { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderTable Order { get; set; }
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
