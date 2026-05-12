using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class InstitutionProfile
    {
        [Key]
        public int InstitutionID { get; set; }

        // User tablosu ile olan ilişki (Foreign Key)
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        public string InstitutionName { get; set; }

        public string EntityType { get; set; } // kurum / sahis

        public string IdentityNumber { get; set; } // Vergi No veya TC kimlik no

        public string TaxCertificatePath { get; set; }

        public string AuthorizedPersonName { get; set; }
        public string AuthorizedPersonPhone { get; set; }
        public string AuthorizedPersonEmail { get; set; }
    }
}