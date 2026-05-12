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
        public decimal Amount { get; set; }

        // Zorunlu olmayan kriterler için soru işareti (?) kullanıyoruz
        public int? DurationMonths { get; set; }
        public int? Quota { get; set; }
        public string GenderCriteria { get; set; }
        public string DepartmentCriteria { get; set; }
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