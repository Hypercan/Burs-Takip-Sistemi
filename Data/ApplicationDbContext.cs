using Microsoft.EntityFrameworkCore;
using BursTakip.Models; // Modellerimizi (User, StudentProfile vb.) kullanabilmek için

namespace BursTakip.Data
{
    // DbContext sınıfından miras alarak bu sınıfın bir veritabanı köprüsü olduğunu belirtiyoruz
    public class ApplicationDbContext : DbContext
    {
        // Bu kurucu metod (constructor), veritabanı bağlantı ayarlarımızı (şifre vs.) içeri alır
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Aşağıdaki her bir satır, Azure'da oluşacak olan tablolarımızın karşılığıdır.
        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<InstitutionProfile> InstitutionProfiles { get; set; }
        public DbSet<ScholarshipProgram> ScholarshipPrograms { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Veritabanındaki tüm tablolar için "Otomatik Silme" (Cascade Delete) özelliğini kapatıyoruz
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}