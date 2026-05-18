using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BursTakip.Models;
using BursTakip.Data;
using System.Security.Claims;

namespace BursTakip.Controllers
{
    // 1. GÜVENLİK: Sadece Kurumlar Girebilir
    [Authorize(Roles = "institution")]
    public class ScholarshipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScholarshipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yardımcı Metot: Giriş yapan kurumun ID'sini bulur
        private int GetCurrentInstitutionId()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var institution = _context.InstitutionProfiles.FirstOrDefault(i => i.UserID == userId);
            return institution?.InstitutionID ?? 0;
        }

        // 1. SADECE KENDİ BURS İLANLARINI LİSTELE
        public IActionResult Index()
        {
            var instId = GetCurrentInstitutionId();
            if (instId == 0) return RedirectToAction("Index", "Institution"); // Profil yoksa profile yolla

            var myScholarships = _context.ScholarshipPrograms
                .Where(s => s.InstitutionID == instId)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(myScholarships);
        }

        // 2. YENİ BURS İLANI OLUŞTURMA SAYFASI GETİR
        public IActionResult Create()
        {
            var instId = GetCurrentInstitutionId();
            if (instId == 0)
            {
                TempData["Error"] = "Burs ilanı açabilmek için önce kurum profilinizi doldurmalısınız.";
                return RedirectToAction("Index", "Institution");
            }
            return View();
        }

        // 3. YENİ BURS İLANINI KAYDET
        [HttpPost]
        public IActionResult Create(ScholarshipProgram model)
        {
            var instId = GetCurrentInstitutionId();
            if (instId == 0) return RedirectToAction("Index", "Institution");

            // Formdan gelmeyen verileri biz arka planda otomatik dolduruyoruz:
            model.InstitutionID = instId;
            model.CreatedAt = DateTime.Now;
            model.Status = "Aktif"; // Varsayılan olarak aktif başlasın
            model.AdminNote = "Yeni oluşturuldu.";

            _context.ScholarshipPrograms.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Yeni burs ilanınız başarıyla yayınlandı!";
            return RedirectToAction(nameof(Index));
        }

        // 4. BURS İLANINI SİL (Güncellendi)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var instId = GetCurrentInstitutionId();
            var scholarship = _context.ScholarshipPrograms.FirstOrDefault(s => s.ProgramID == id && s.InstitutionID == instId);

            if (scholarship != null)
            {
                // Önce bu bursa yapılmış tüm başvuruları bul ve sil (Veritabanı ilişkisel hatasını önlemek için)
                var relatedApplications = _context.Applications.Where(a => a.ProgramID == id).ToList();
                if (relatedApplications.Any())
                {
                    _context.Applications.RemoveRange(relatedApplications);
                }

                // Bağlı başvurular silindikten sonra bursu güvenle silebiliriz
                _context.ScholarshipPrograms.Remove(scholarship);
                _context.SaveChanges();
                TempData["Success"] = "Burs ilanı ve ona bağlı tüm başvurular başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // 5. BİR BURSUN BAŞVURULARINI LİSTELE
        public IActionResult Applications(int id)
        {
            var instId = GetCurrentInstitutionId();
            
            // Önce bursun bu kuruma ait olduğundan emin olalım (güvenlik)
            var program = _context.ScholarshipPrograms.FirstOrDefault(p => p.ProgramID == id && p.InstitutionID == instId);
            if (program == null) return NotFound();

            ViewBag.ProgramName = program.ProgramName;

            // Bu bursa yapılmış başvuruları ve başvuran öğrencilerin profillerini çekiyoruz
            var applications = _context.Applications
                .Include(a => a.Student)
                // Sadece statüsü "Beklemede" olan başvuruları getiriyoruz
                .Where(a => a.ProgramID == id && a.Status == "Beklemede")
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            // Öğrencilerin sisteme yüklediği belgeleri de güvenli yoldan çekip ViewBag ile ekrana yolluyoruz
            var studentIds = applications.Select(a => a.StudentID).ToList();
            ViewBag.StudentDocuments = _context.Documents.Where(d => studentIds.Contains(d.StudentID)).ToList();

            return View(applications);
        }

        // 6. BAŞVURU ONAY / RED İŞLEMİ
        [HttpPost]
        public IActionResult UpdateStatus(int applicationId, string status, string institutionNote)
        {
            var instId = GetCurrentInstitutionId();
            
            // Başvuruyu bulurken, bursun bizim kuruma ait olduğunu da teyit ediyoruz
            var application = _context.Applications
                .Include(a => a.Program)
                .FirstOrDefault(a => a.ApplicationID == applicationId && a.Program.InstitutionID == instId);

            if (application != null)
            {
                application.Status = status; // "Onaylandı" veya "Reddedildi"
                application.InstitutionNote = string.IsNullOrWhiteSpace(institutionNote) ? "Değerlendirildi." : institutionNote;
                application.UpdatedAt = DateTime.Now;
                
                _context.SaveChanges();
                TempData["Success"] = "Öğrencinin başvuru durumu başarıyla güncellendi.";
                
                return RedirectToAction("Applications", new { id = application.ProgramID });
            }

            return NotFound();
        }
    }
}