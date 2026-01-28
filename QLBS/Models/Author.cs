using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Author")]
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int? BirthYear { get; set; }
        public string? Nationality { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
