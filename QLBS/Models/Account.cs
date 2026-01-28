using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBS.Models
{
    [Table("Account")]
    public class Account
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }

        public string? RefreshToken { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public string? OTP { get; set; }
        public DateTime? OtpExpires { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
