using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class SystemLog
    {
        [Key]
        public int LogID { get; set; }

        // User tablosu ile olan ilişki (Bu işlemi hangi kullanıcı yaptı?)
        // Sisteme giriş yapmamış biri de hata alabileceği için nullable (?) yapıyoruz
        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        public string Action { get; set; } // Örn: "Sisteme giriş yaptı", "Burs başvurusu reddedildi"

        public string IPAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string Details { get; set; } // Hatanın veya işlemin uzun detayı
    }
}