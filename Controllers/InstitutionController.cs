using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BursTakip.Models;
using BursTakip.Data;
using System.Security.Claims;

namespace BursTakip.Controllers
{
    [Authorize(Roles = "institution")] // Sadece kurumlar girebilir
    public class InstitutionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstitutionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. KURUM PROFİLİNİ GETİR
        [HttpGet]
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = _context.InstitutionProfiles.FirstOrDefault(i => i.UserID == userId);

            // Profil varsa göster, yoksa boş bir profil formu gönder
            return View(profile ?? new InstitutionProfile());
        }

        // 2. KURUM PROFİLİNİ KAYDET VEYA GÜNCELLE
        [HttpPost]
        public IActionResult Index(InstitutionProfile model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existingProfile = _context.InstitutionProfiles.FirstOrDefault(i => i.UserID == userId);

            if (existingProfile == null)
            {
                // İlk defa kaydediyorsa
                model.UserID = userId;
                _context.InstitutionProfiles.Add(model);
                ViewBag.Message = "Kurum profiliniz başarıyla oluşturuldu!";
            }
            else
            {
                // Mevcut profili güncelliyorsa
                existingProfile.InstitutionName = model.InstitutionName;
                existingProfile.EntityType = model.EntityType;
                existingProfile.IdentityNumber = model.IdentityNumber;
                existingProfile.AuthorizedPersonName = model.AuthorizedPersonName;
                existingProfile.AuthorizedPersonPhone = model.AuthorizedPersonPhone;
                existingProfile.AuthorizedPersonEmail = model.AuthorizedPersonEmail;
                // TaxCertificatePath (Vergi Levhası) dosya yükleme işlemi sonraya bırakıldı.
                
                _context.InstitutionProfiles.Update(existingProfile);
                ViewBag.Message = "Kurum profiliniz başarıyla güncellendi!";
            }

            _context.SaveChanges();
            return View(existingProfile ?? model);
        }
    }
}