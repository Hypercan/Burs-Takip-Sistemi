using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class Application
    {
        [Key]
        public int ApplicationID { get; set; }

        // StudentProfiles tablosu ile olan ilişki (Hangi öğrenci başvurdu?)
        public int StudentID { get; set; }
        [ForeignKey("StudentID")]
        public StudentProfile Student { get; set; }

        // ScholarshipPrograms tablosu ile olan ilişki (Hangi bursa başvurdu?)
        public int ProgramID { get; set; }
        [ForeignKey("ProgramID")]
        public ScholarshipProgram Program { get; set; }

        public string Status { get; set; } // beklemede / incelemede / revizyon / kabul / red

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Başvuru ilk yapıldığında güncellenme tarihi olmayacağı için nullable (?) yapıyoruz
        public DateTime? UpdatedAt { get; set; }

        public string InstitutionNote { get; set; }
    }
}