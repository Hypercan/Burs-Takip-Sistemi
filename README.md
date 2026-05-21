# Burs Takip Sistemi (BURSTAR)

Burs veren kurumlar ile burs arayan öğrenciler arasındaki süreci tek bir web platformunda yöneten **ASP.NET Core MVC** uygulamasıdır. Aktif burs ilanlarını vitrinde listeler; öğrenciler profil ve belgeleriyle başvuru yapar; kurumlar ilan oluşturup başvuruları değerlendirir; yönetici genel istatistikleri izler.

---

## İçindekiler

- [Özellikler](#özellikler)
- [Teknoloji yığını](#teknoloji-yığını)
- [Gereksinimler](#gereksinimler)
- [Kurulum](#kurulum)
- [Veritabanı](#veritabanı)
- [Demo verileri](#demo-verileri)
- [Proje yapısı](#proje-yapısı)
- [Dokümantasyon](#dokümantasyon)
- [Güvenlik notları](#güvenlik-notları)

---

## Özellikler

### Herkes (ziyaretçi ve giriş yapmış kullanıcılar)

- Aktif ve son başvuru tarihi geçmemiş burs ilanlarını ana sayfada görüntüleme
- Burs ilanı detaylarını inceleme (kurum, tutar, süre, kriterler, tarihler)
- Kayıt olma ve oturum açma

### Öğrenci (`student`)

- Profil oluşturma ve güncelleme (kişisel, akademik, iletişim, IBAN)
- Belge yükleme (transkript, kimlik vb.)
- Aktif burs programlarına **tek seferlik** başvuru
- Başvuruları “Başvurularım” ekranında takip etme

### Kurum (`institution`)

- Kurum profili tanımlama
- Burs programı / ilan oluşturma ve yönetme
- Gelen başvuruları görüntüleme ve onaylama / reddetme

### Yönetici (`admin`)

- Özet gösterge paneli (kullanıcı, öğrenci, kurum, burs ve başvuru sayıları)
- Kullanıcı listesi ve rol dağılımı

---

## Teknoloji yığını

| Katman | Teknoloji |
|--------|-----------|
| Uygulama | ASP.NET Core 10 (MVC) |
| Veri erişimi | Entity Framework Core 10 (Code First) |
| Veritabanı | Microsoft SQL Server |
| Kimlik doğrulama | Cookie tabanlı oturum |
| Arayüz | Razor Views, Bootstrap |

---

## Gereksinimler

- [.NET SDK 10](https://dotnet.microsoft.com/download) veya üzeri
- SQL Server (yerel, Docker veya Azure SQL)
- (İsteğe bağlı) [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

---

## Kurulum

### 1. Depoyu klonlayın

```bash
git clone <repo-url>
cd Burs-Takip-Sistemi
```

### 2. Bağımlılıkları yükleyin

```bash
dotnet restore
```

### 3. Veritabanı bağlantısını ayarlayın

`appsettings.json` içindeki `ConnectionStrings:DefaultConnection` değerini kendi SQL Server bilgilerinizle güncelleyin.

**Öneri:** Geliştirme ortamında parolayı depoya yazmayın; [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) kullanın:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=BursDb;..."
```

Proje zaten bir `UserSecretsId` tanımlıdır (`Burs-Takip-Sistemi.csproj`).

### 4. Veritabanını oluşturun

```bash
dotnet ef database update
```

### 5. Uygulamayı çalıştırın

```bash
dotnet run
```

Tarayıcıda genelde `https://localhost:5xxx` veya `http://localhost:5xxx` adresi açılır (çıktıdaki URL’yi kullanın).

---

## Veritabanı

- Şema **Code First migration** ile yönetilir (`Migrations/` klasörü).
- İlk migration: `20260512173712_InitialCreate`
- Manuel kurulum için T-SQL script: `docs/sql/01_CreateSchema.sql`

Ana tablolar: `Users`, `StudentProfiles`, `InstitutionProfiles`, `ScholarshipPrograms`, `Applications`, `Documents`, `ApplicationDocuments`, `SystemLogs`.

Detaylı ER diyagramı, alan açıklamaları ve kurulum adımları için bkz. [docs/VERITABANI_TASARIM_VE_KURULUM.md](docs/VERITABANI_TASARIM_VE_KURULUM.md).

---

## Demo verileri

**Development** ortamında uygulama ilk çalıştığında `DemoDataSeeder` otomatik olarak örnek kurumlar ve burs ilanları ekler (daha önce eklenmemişse).

| Bilgi | Değer |
|-------|--------|
| Demo kurum e-postaları | `tev@demo.burstari.local`, `sabanci@demo.burstari.local`, `anadolu@demo.burstari.local`, `kged@demo.burstari.local` |
| Demo şifre | `Demo123!` |

Öğrenci veya yönetici denemek için **Kayıt Ol** ekranından yeni hesap oluşturabilirsiniz. Yönetici paneli için veritabanında `Role = admin` olan bir kullanıcı gerekir (kayıt formunda yalnızca `student` ve `institution` seçilebilir).

---

## Proje yapısı

```
Burs-Takip-Sistemi/
├── Controllers/          # MVC denetleyicileri (Auth, Student, Institution, Scholarship, Application, Admin)
├── Data/                 # ApplicationDbContext, DemoDataSeeder
├── Models/               # Varlık modelleri
├── Migrations/           # EF Core migration dosyaları
├── Views/                # Razor görünümleri (rol bazlı klasörler)
├── wwwroot/              # CSS, JS, yüklenen belgeler (uploads/)
├── docs/                 # Analiz, veritabanı ve SQL belgeleri
├── Program.cs            # Uygulama giriş noktası ve middleware
└── appsettings.json      # Bağlantı dizesi ve log ayarları
```

### Denetleyiciler (özet)

| Denetleyici | Görev |
|-------------|--------|
| `HomeController` | Ana sayfa, aktif burs vitrini |
| `AuthController` | Kayıt, giriş, çıkış |
| `StudentController` | Öğrenci profili ve belgeler |
| `InstitutionController` | Kurum profili |
| `ScholarshipController` | Burs ilanları (kurum tarafı) |
| `ApplicationController` | Başvuru oluşturma ve değerlendirme |
| `AdminController` | Yönetici paneli |

---

## Dokümantasyon

| Belge | İçerik |
|-------|--------|
| [docs/ANALIZ_VE_TASARIM_BELGESI.md](docs/ANALIZ_VE_TASARIM_BELGESI.md) | Gereksinimler, roller, kullanım senaryoları, mimari özet |
| [docs/VERITABANI_TASARIM_VE_KURULUM.md](docs/VERITABANI_TASARIM_VE_KURULUM.md) | Tablolar, ilişkiler, EF ve SQL kurulumu |
| [docs/sql/01_CreateSchema.sql](docs/sql/01_CreateSchema.sql) | Manuel şema oluşturma scripti |

---

## Güvenlik notları

- `appsettings.json` içine **gerçek üretim parolaları** commit etmeyin; User Secrets veya ortam değişkenleri kullanın.
- Şifreler veritabanında hash olarak saklanır (SHA-256).
- Oturum çerezi adı: `BursTakipCookie` (varsayılan süre: 7 gün).
- Yüklenen belgeler `wwwroot/uploads/` altında benzersiz dosya adlarıyla tutulur.

---

## Lisans

Bu proje akademik bir yazılım mühendisliği çalışması olarak geliştirilmiştir. Lisans bilgisi depoda ayrıca belirtilmemişse kullanım için proje sahibiyle iletişime geçin.
