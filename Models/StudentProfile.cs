using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BursTakip.Models
{
    public class StudentProfile
    {
        [Key]
        public int StudentID { get; set; }

        // User tablosu ile olan ilişki (Foreign Key)
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public string Gender { get; set; }

        public bool DisabilityStatus { get; set; } = false; // Veritabanındaki 'bit' C#'ta 'bool' (true/false) olur

        public string Department { get; set; }
        public string School { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }
        public string PhotoPath { get; set; }
    }
}