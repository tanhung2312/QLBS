using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("OrderTable")]
    public class OrderTable
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalQuantity { get; set; }

        public byte OrderStatus { get; set; }
        public int? DiscountCodeId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("DiscountCodeId")]
        public virtual DiscountCode DiscountCode { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
