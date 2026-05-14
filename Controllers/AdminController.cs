using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BursTakip.Data;

namespace BursTakip.Controllers
{
    // Sadece "admin" rolündekiler girebilir!
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dashboard için hızlı istatistikler topluyoruz
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalStudents = _context.StudentProfiles.Count();
            ViewBag.TotalInstitutions = _context.InstitutionProfiles.Count();
            ViewBag.TotalScholarships = _context.ScholarshipPrograms.Count();
            ViewBag.TotalApplications = _context.Applications.Count();

            // Son kayıt olan 5 kullanıcıyı listeye alalım
            var recentUsers = _context.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToList();

            return View(recentUsers);
        }

        // Kullanıcıları listeleme ve yönetme ekranı (İleride geliştirilebilir)
        public IActionResult UserManagement()
        {
            var allUsers = _context.Users.OrderByDescending(u => u.CreatedAt).ToList();
            return View(allUsers);
        }
    }
}