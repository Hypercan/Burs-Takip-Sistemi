using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BursTakip.Models;
using BursTakip.Data;
using System.Security.Claims;

namespace BursTakip.Controllers
{
    // Bu sayfaya sadece giriş yapmış ve rolü "student" olanlar girebilir!
    [Authorize(Roles = "student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ÖĞRENCİ PROFİL SAYFASINI GETİR
        [HttpGet]
        public IActionResult Index()
        {
            // Sisteme giriş yapmış kullanıcının ID'sini alıyoruz
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Veritabanında bu kullanıcıya ait bir profil var mı bakıyoruz
            var studentProfile = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            // Profil varsa onu ekrana gönder, yoksa boş bir profil gönder
            return View(studentProfile ?? new StudentProfile());
        }

        // 2. ÖĞRENCİ PROFİLİNİ KAYDET VEYA GÜNCELLE
        [HttpPost]
        public IActionResult Index(StudentProfile model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Veritabanında profil var mı kontrol et
            var existingProfile = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            if (existingProfile == null)
            {
                // İlk defa profil oluşturuluyor
                model.UserID = userId;
                _context.StudentProfiles.Add(model);
                ViewBag.Message = "Profiliniz başarıyla oluşturuldu!";
            }
            else
            {
                // Mevcut profil güncelleniyor
                existingProfile.FirstName = model.FirstName;
                existingProfile.LastName = model.LastName;
                existingProfile.BirthDate = model.BirthDate;
                existingProfile.Gender = model.Gender;
                existingProfile.Department = model.Department;
                existingProfile.School = model.School;
                existingProfile.Phone = model.Phone;
                existingProfile.IBAN = model.IBAN;
                
                _context.StudentProfiles.Update(existingProfile);
                ViewBag.Message = "Profiliniz başarıyla güncellendi!";
            }

            _context.SaveChanges();
            return View(existingProfile ?? model);
        }

        // 3. ÖĞRENCİ BELGELERİM SAYFASINI GETİR
        [HttpGet]
        public IActionResult Documents()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            if (student == null)
            {
                // Öğrenci henüz profil bilgilerini doldurmamışsa, belge yükleyemez. Profile geri yolla.
                TempData["Error"] = "Belge yükleyebilmek için önce profilinizi kaydetmelisiniz.";
                return RedirectToAction("Index");
            }

            // Veritabanından bu öğrencinin daha önce yüklediği belgeleri çekiyoruz
            var documents = _context.Documents.Where(d => d.StudentID == student.StudentID).ToList();

            return View(documents);
        }

        // 4. BELGE YÜKLEME (UPLOAD) İŞLEMİ
        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file, string documentType)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            if (student == null) return RedirectToAction("Index");

            if (file != null && file.Length > 0)
            {
                // Dosyanın fiziksel olarak kaydedileceği klasör (wwwroot/uploads/documents)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                
                // Eğer böyle bir klasör henüz yoksa, otomatik oluştur
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Dosya adının benzersiz olması için başına GUID (karmaşık şifre) ekliyoruz
                // Böylece iki kişi "transkript.pdf" yüklerse birbirinin dosyasını ezmez
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Dosyayı klasöre kopyala
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Dosyanın adresini ve türünü veritabanına kaydet
                var newDocument = new Document
                {
                    StudentID = student.StudentID,
                    DocumentType = documentType,
                    FilePath = "/uploads/documents/" + uniqueFileName,
                    UploadedAt = DateTime.Now
                };

                _context.Documents.Add(newDocument);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Belgeniz başarıyla yüklendi!";
            }

            return RedirectToAction("Documents");
        }
    }
}