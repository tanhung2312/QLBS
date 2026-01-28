using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("DiscountCode")]
    public class DiscountCode
    {
        public int DiscountCodeId { get; set; }
        public string Code { get; set; }

        public byte DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Quantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<OrderTable> Orders { get; set; }
    }
}
