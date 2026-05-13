using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BursTakip.Models;
using BursTakip.Data;
using System.Security.Cryptography;
using System.Text;

namespace BursTakip.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Veritabanı bağlantımızı Controller'a alıyoruz
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. KAYIT OL EKRANINI GETİR
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. KAYIT OL FORMUNU GÖNDER
        [HttpPost]
        public IActionResult Register(string email, string password, string role)
        {
            // Bu email daha önce kullanılmış mı?
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Bu email adresi sistemde zaten kayıtlı!";
                return View();
            }

            // Yeni kullanıcıyı oluştur
            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password), // Şifreyi şifreleyerek kaydediyoruz
                Role = role, // "student" veya "institution"
                ApprovalStatus = "onaylandi", // Şimdilik test için direkt onaylı yapıyoruz
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // 3. GİRİŞ YAP EKRANINI GETİR
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 4. GİRİŞ YAP FORMUNU GÖNDER
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            
            // Veritabanında bu email ve şifreye sahip kullanıcı var mı?
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Email veya şifre hatalı!";
                return View();
            }

            // Kullanıcı doğruysa ona bir "Kimlik Kartı" (Cookie) oluşturuyoruz
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sisteme giriş yaptır
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Giriş başarılıysa Ana Sayfaya gönder
            return RedirectToAction("Index", "Home");
        }

        // 5. ÇIKIŞ YAP
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Güvenlik: Şifreyi açık metin olarak değil, SHA256 ile karıştırarak kaydetmek için basit bir metot
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}