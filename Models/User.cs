using System;
using System.ComponentModel.DataAnnotations;

namespace BursTakip.Models
{
    public class User
    {
        [Key] // Primary Key (PK) olduğunu belirtir
        public int UserID { get; set; }

        [Required] // Not null (boş geçilemez) olduğunu belirtir
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } // student / institution / admin

        public string ApprovalStatus { get; set; } // beklemede / onaylandi / reddedildi

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 'now()' karşılığı
    }
}