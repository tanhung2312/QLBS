using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("BookImage")]
    public class BookImage
    {
        [Key]
        public int ImageId { get; set; }
        public int BookId { get; set; }

        public string URL { get; set; }
        public string? Description { get; set; }
        public int OrderIndex { get; set; }
        public bool IsCover { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
