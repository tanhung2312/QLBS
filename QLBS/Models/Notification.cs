using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Notification")]
    public class Notification
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }
        public int UserId { get; set; }

        public string Content { get; set; }
        public string? URL { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationType { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
    }
}
