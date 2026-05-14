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

        // 4. BURS İLANINI SİL
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var instId = GetCurrentInstitutionId();
            var scholarship = _context.ScholarshipPrograms.FirstOrDefault(s => s.ProgramID == id && s.InstitutionID == instId);

            if (scholarship != null)
            {
                _context.ScholarshipPrograms.Remove(scholarship);
                _context.SaveChanges();
                TempData["Success"] = "Burs ilanı başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}