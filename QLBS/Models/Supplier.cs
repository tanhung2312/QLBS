using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual ICollection<BookReceipt> BookReceipts { get; set; }
    }
}
