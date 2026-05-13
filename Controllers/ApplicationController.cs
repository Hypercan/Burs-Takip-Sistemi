using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BursTakip.Models;
using BursTakip.Data;
using System.Security.Claims;

namespace BursTakip.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. BURS DETAYLARINI GÖSTER (Giriş yapmayanlar bile görebilir)
        public IActionResult Details(int id)
        {
            var scholarship = _context.ScholarshipPrograms
                .Include(s => s.Institution)
                .FirstOrDefault(s => s.ProgramID == id);

            if (scholarship == null) return NotFound();

            return View(scholarship);
        }

        // 2. BAŞVURU YAPMA İŞLEMİ (Sadece "Öğrenci" rolündekiler yapabilir)
        [Authorize(Roles = "student")]
        [HttpPost]
        public IActionResult Apply(int programId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            // Öğrenci profilini henüz doldurmamışsa başvuramaz
            if (student == null)
            {
                TempData["Error"] = "Başvuru yapabilmek için önce profil bilgilerinizi doldurmalısınız.";
                return RedirectToAction("Index", "Student");
            }

            // Öğrenci bu bursa daha önce başvurmuş mu kontrolü
            var hasApplied = _context.Applications.Any(a => a.ProgramID == programId && a.StudentID == student.StudentID);
            if (hasApplied)
            {
                TempData["Error"] = "Bu bursa zaten başvuru yaptınız!";
                return RedirectToAction("Details", new { id = programId });
            }

            // Yeni Başvuruyu oluştur ve veritabanına kaydet
            var application = new Application
            {
                StudentID = student.StudentID,
                ProgramID = programId,
                Status = "Beklemede", // İlk başvuru her zaman beklemede başlar
                AppliedAt = DateTime.Now,
                InstitutionNote = "Henüz değerlendirilmedi."
            };

            _context.Applications.Add(application);
            _context.SaveChanges();

            TempData["Success"] = "Tebrikler! Başvurunuz başarıyla alındı.";
            return RedirectToAction("MyApplications");
        }

        // 3. ÖĞRENCİNİN KENDİ BAŞVURULARINI GÖRDÜĞÜ SAYFA
        [Authorize(Roles = "student")]
        public IActionResult MyApplications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var student = _context.StudentProfiles.FirstOrDefault(s => s.UserID == userId);

            if (student == null) return RedirectToAction("Index", "Student");

            // Öğrencinin başvurularını, burs ve kurum bilgileriyle beraber çekiyoruz
            var myApplications = _context.Applications
                .Include(a => a.Program)
                .ThenInclude(p => p.Institution)
                .Where(a => a.StudentID == student.StudentID)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            return View(myApplications);
        }
    }
}