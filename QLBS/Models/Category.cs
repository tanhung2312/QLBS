using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Categorie")]
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
