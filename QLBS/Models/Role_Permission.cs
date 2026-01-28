using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Role_Permission")]
    public class Role_Permission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}
