using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Permission")]
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Code { get; set; }
        public string PermissionName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Role_Permission> RolePermissions { get; set; }
    }
}
