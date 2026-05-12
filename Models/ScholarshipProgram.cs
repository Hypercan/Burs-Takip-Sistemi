using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class ScholarshipProgram
    {
        [Key]
        public int ProgramID { get; set; }

        // InstitutionProfiles tablosu ile olan ilişki (Foreign Key)
        public int InstitutionID { get; set; }
        [ForeignKey("InstitutionID")]
        public InstitutionProfile Institution { get; set; }

        [Required]
        public string ProgramName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Toplam 18 rakam, virgülden sonra 2 hane (Örn: 1500,50)
        public decimal Amount { get; set; }

        // Zorunlu olmayan kriterler için soru işareti (?) kullanıyoruz
        public int? DurationMonths { get; set; }
        public int? Quota { get; set; }
        public string GenderCriteria { get; set; }
        public string DepartmentCriteria { get; set; }
        [Column(TypeName = "decimal(4,2)")] // Toplam 4 rakam, virgülden sonra 2 hane (Örn: 3,50)
        public decimal? MinGPA { get; set; }

        public string Status { get; set; } // taslak / onay_bekliyor / aktif / kapali / reddedildi

        public DateTime ApplicationDeadline { get; set; }
        public DateTime SubmissionDeadline { get; set; }

        public string AdminNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Tarihler başlangıçta boş olabileceği için nullable (?) yapıyoruz
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}