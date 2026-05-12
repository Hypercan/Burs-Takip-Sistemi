using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }

        // StudentProfiles tablosu ile olan ilişki (Bu belge hangi öğrenciye ait?)
        public int StudentID { get; set; }
        [ForeignKey("StudentID")]
        public StudentProfile Student { get; set; }

        public string DocumentType { get; set; } // transkript / ogrenci_belgesi / kimlik / adli_sicil / nufus_ornegi

        [Required]
        public string FilePath { get; set; } // Dosyanın fiziksel olarak nerede durduğu

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}