using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BursTakip.Data;
using BursTakip.Models;

namespace BursTakip.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Veritabanından sadece "Aktif" olan ve son başvuru tarihi geçmemiş bursları çekiyoruz.
            // .Include(s => s.Institution) kodu, bursa bağlı olan Kurumun adını da almamızı sağlar.
            var activeScholarships = _context.ScholarshipPrograms
                .Include(s => s.Institution) 
                .Where(s => s.Status == "Aktif" && s.ApplicationDeadline >= DateTime.Now)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(activeScholarships);
        }

        // Hata sayfası için standart metod (buna dokunmuyoruz)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}