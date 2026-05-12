using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class ApplicationDocument
    {
        [Key]
        public int AppDocID { get; set; }

        // Applications tablosu ile olan ilişki (Hangi başvuru için eklendi?)
        public int ApplicationID { get; set; }
        [ForeignKey("ApplicationID")]
        public Application Application { get; set; }

        // Documents tablosu ile olan ilişki (Eklenen belge hangisi?)
        public int DocumentID { get; set; }
        [ForeignKey("DocumentID")]
        public Document Document { get; set; }

        public string Status { get; set; } // beklemede / onaylandi / reddedildi

        public DateTime? ReviewedAt { get; set; }

        // Hangi Admin veya Kurum yetkilisi inceledi? (User tablosuna referans)
        public int? ReviewedByID { get; set; }
        [ForeignKey("ReviewedByID")]
        public User ReviewedBy { get; set; }
    }
}