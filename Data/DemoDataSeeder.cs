using BursTakip.Models;
using System.Security.Cryptography;
using System.Text;

namespace BursTakip.Data
{
    /// <summary>
    /// Geliştirme ve demo ortamı için gerçekçi kurum ve burs ilanları ekler.
    /// demo.burstari.local e-postalı kayıtlar zaten varsa tekrar eklemez.
    /// </summary>
    public static class DemoDataSeeder
    {
        private const string DemoEmailDomain = "@demo.burstari.local";
        private const string DemoPassword = "Demo123!";

        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any(u => u.Email.EndsWith(DemoEmailDomain)))
                return;

            var passwordHash = HashPassword(DemoPassword);
            var now = DateTime.Now;

            var institutions = new[]
            {
                new DemoInstitution(
                    "tev@demo.burstari.local",
                    "Türk Eğitim Vakfı (TEV)",
                    "1234567890",
                    "Ayşe Yılmaz",
                    "+90 212 555 0101",
                    "tev@demo.burstari.local",
                    new[]
                    {
                        Scholarship("TEV Üstün Başarı Bursu 2026", 4500, 10, 40, "Farketmez", "Mühendislik, Fen ve Matematik Bölümleri", 3.20m, new DateTime(2026, 9, 30)),
                        Scholarship("TEV Kadın Mühendislik Bursu", 5000, 12, 25, "Sadece Kadın", "Bilgisayar, Elektrik-Elektronik, Makine Mühendisliği", 2.80m, new DateTime(2026, 8, 15)),
                        Scholarship("TEV Sosyal Bilimler Destek Bursu", 3500, 8, 30, "Farketmez", "Psikoloji, Sosyoloji, Tarih, Felsefe", 2.50m, new DateTime(2026, 10, 1)),
                        Scholarship("TEV Lise Mezunu Üniversite Geçiş Bursu", 3000, 6, 50, "Farketmez", "Tüm Lisans Bölümleri", 2.00m, new DateTime(2026, 7, 20))
                    }),
                new DemoInstitution(
                    "sabanci@demo.burstari.local",
                    "Sabancı Vakfı",
                    "2345678901",
                    "Mehmet Kaya",
                    "+90 216 555 0202",
                    "sabanci@demo.burstari.local",
                    new[]
                    {
                        Scholarship("Sabancı Girişimci Gençler Bursu", 6000, 12, 20, "Farketmez", "İşletme, İktisat, Endüstri Mühendisliği", 3.00m, new DateTime(2026, 11, 15)),
                        Scholarship("Sabancı Sanat ve Tasarım Bursu", 4000, 9, 15, "Farketmez", "Güzel Sanatlar, Mimarlık, İç Mimarlık", 2.75m, new DateTime(2026, 9, 1)),
                        Scholarship("Sabancı Erkek Sporcu Öğrenci Bursu", 3800, 8, 12, "Sadece Erkek", "Beden Eğitimi ve Spor, Spor Bilimleri", 2.60m, new DateTime(2026, 8, 30)),
                        Scholarship("Sabancı Uluslararası İlişkiler Bursu", 5500, 10, 18, "Farketmez", "Uluslararası İlişkiler, Siyaset Bilimi", 3.10m, new DateTime(2026, 12, 1))
                    }),
                new DemoInstitution(
                    "anadolu@demo.burstari.local",
                    "Anadolu Üniversiteler Birliği",
                    "3456789012",
                    "Zeynep Demir",
                    "+90 312 555 0303",
                    "anadolu@demo.burstari.local",
                    new[]
                    {
                        Scholarship("AÜB Tıp Fakültesi Bursu", 7500, 12, 10, "Farketmez", "Tıp, Diş Hekimliği, Eczacılık", 3.50m, new DateTime(2026, 10, 20)),
                        Scholarship("AÜB Hukuk ve Kamu Yönetimi Bursu", 4200, 10, 22, "Farketmez", "Hukuk, Kamu Yönetimi, Siyaset Bilimi", 3.00m, new DateTime(2026, 9, 15)),
                        Scholarship("AÜB Kadın Sağlık Bilimleri Bursu", 4800, 11, 20, "Sadece Kadın", "Hemşirelik, Ebelik, Fizyoterapi", 2.90m, new DateTime(2026, 8, 25)),
                        Scholarship("AÜB Tarım ve Veteriner Bursu", 3600, 8, 16, "Farketmez", "Ziraat, Veteriner, Gıda Mühendisliği", 2.70m, new DateTime(2026, 11, 30))
                    }),
                new DemoInstitution(
                    "kged@demo.burstari.local",
                    "Kadın ve Gençlik Eğitim Derneği (KGED)",
                    "4567890123",
                    "Elif Arslan",
                    "+90 232 555 0404",
                    "kged@demo.burstari.local",
                    new[]
                    {
                        Scholarship("KGED Kadın Liderlik Bursu", 5200, 12, 30, "Sadece Kadın", "Tüm Lisans Bölümleri", 2.80m, new DateTime(2026, 10, 10)),
                        Scholarship("KGED Erkek Öğretmen Adayı Bursu", 3400, 8, 15, "Sadece Erkek", "Öğretmenlik, Eğitim Bilimleri, PDR", 2.65m, new DateTime(2026, 7, 31)),
                        Scholarship("KGED İletişim ve Medya Bursu", 4100, 9, 20, "Farketmez", "Gazetecilik, Radyo-TV, Yeni Medya", 2.85m, new DateTime(2026, 9, 20)),
                        Scholarship("KGED Mimarlık ve Şehir Planlama Bursu", 4700, 10, 12, "Farketmez", "Mimarlık, Şehir ve Bölge Planlama, Peyzaj", 3.00m, new DateTime(2026, 11, 1))
                    })
            };

            foreach (var demo in institutions)
            {
                var user = new User
                {
                    Email = demo.Email,
                    PasswordHash = passwordHash,
                    Role = "institution",
                    ApprovalStatus = "onaylandi",
                    CreatedAt = now
                };
                context.Users.Add(user);
                context.SaveChanges();

                var profile = new InstitutionProfile
                {
                    UserID = user.UserID,
                    InstitutionName = demo.Name,
                    EntityType = "kurum",
                    IdentityNumber = demo.TaxId,
                    TaxCertificatePath = "/uploads/demo/vergi-levhasi.pdf",
                    AuthorizedPersonName = demo.ContactName,
                    AuthorizedPersonPhone = demo.Phone,
                    AuthorizedPersonEmail = demo.ContactEmail
                };
                context.InstitutionProfiles.Add(profile);
                context.SaveChanges();

                foreach (var s in demo.Scholarships)
                {
                    context.ScholarshipPrograms.Add(new ScholarshipProgram
                    {
                        InstitutionID = profile.InstitutionID,
                        ProgramName = s.Name,
                        Amount = s.Amount,
                        DurationMonths = s.Months,
                        Quota = s.Quota,
                        GenderCriteria = s.Gender,
                        DepartmentCriteria = s.Department,
                        MinGPA = s.MinGpa,
                        Status = "Aktif",
                        ApplicationDeadline = s.Deadline,
                        SubmissionDeadline = s.Deadline.AddDays(15),
                        AdminNote = "Demo vitrin verisi.",
                        CreatedAt = now.AddDays(-Random.Shared.Next(1, 30)),
                        ApprovedAt = now
                    });
                }
            }

            context.SaveChanges();
        }

        private static (string Name, decimal Amount, int Months, int Quota, string Gender, string Department, decimal? MinGpa, DateTime Deadline) Scholarship(
            string name, decimal amount, int months, int quota, string gender, string department, decimal? minGpa, DateTime deadline)
            => (name, amount, months, quota, gender, department, minGpa, deadline);

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private sealed record DemoInstitution(
            string Email,
            string Name,
            string TaxId,
            string ContactName,
            string Phone,
            string ContactEmail,
            (string Name, decimal Amount, int Months, int Quota, string Gender, string Department, decimal? MinGpa, DateTime Deadline)[] Scholarships);
    }
}
