using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("FavoriteBook")]
    public class FavoriteBook
    {
        public int UserId { get; set; }
        public int BookId { get; set; }

        public DateTime MarkedDate { get; set; }
        public bool IsDeleted { get; set; }
       
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
