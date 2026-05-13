using Microsoft.EntityFrameworkCore;
using BursTakip.Data;
using Microsoft.AspNetCore.Authentication.Cookies; // Bizim eklediğimiz

var builder = WebApplication.CreateBuilder(args);

// 1. SERVİSLER BURAYA EKLENİR (Build'den ÖNCE)
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(180)));

// Kimlik Doğrulama (Cookie) Ayarları BURADA OLMALI
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "BursTakipCookie";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// --- MOTOR BURADA KİLİTLENİR ---
var app = builder.Build();

// 2. MIDDLEWARE (ARAYAZILIM) AYARLARI BURAYA EKLENİR (Build'den SONRA)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Bizim eklediğimiz (Authorization'dan önce olmalı)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();