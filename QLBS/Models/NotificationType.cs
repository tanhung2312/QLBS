using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("NotificationType")]
    public class NotificationType
    {
        public int NotificationTypeId { get; set; }
        public string NotificationTypeName { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
