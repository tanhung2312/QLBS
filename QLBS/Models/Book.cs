using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Book")]
    public class Book
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }

        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public int PublishYear { get; set; }
        public string Publisher { get; set; }
        public DateTime EntryDate { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        public virtual ICollection<BookImage> BookImages { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
