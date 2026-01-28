using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Role")]
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Role_Permission> RolePermissions { get; set; }
    }
}
