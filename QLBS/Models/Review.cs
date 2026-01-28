using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Review")]
    public class Review
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }

        public byte Rating { get; set; }
        public string Comment { get; set; }
        public byte Status { get; set; }

        public DateTime ReviewDate { get; set; }
        public string? Note { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
