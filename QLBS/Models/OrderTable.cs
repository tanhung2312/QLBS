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

        [Required]
        [MaxLength(200)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ShippingAddress { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public int TotalQuantity { get; set; }

        public byte OrderStatus { get; set; }

        public int? DiscountCodeId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingFee { get; set; } = 0;

        [MaxLength(50)]
        public string? GhnOrderCode { get; set; }

        public int? ProvinceID { get; set; } 
        public int? DistrictID { get; set; } 

        [MaxLength(50)]
        public string? WardCode { get; set; }


        [ForeignKey("UserId")]
        public virtual UserProfile? User { get; set; }

        [ForeignKey("DiscountCodeId")]
        public virtual DiscountCode? DiscountCode { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}