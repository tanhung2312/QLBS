using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Cart")]
    public class Cart
    {
        public int UserId { get; set; }
        public int BookId { get; set; }

        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
