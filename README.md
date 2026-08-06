# BackendMarket

Müşteri, satıcı ve yönetici akışlarını aynı uygulamada birleştiren tam yığın e-ticaret platformu. Ürün kataloğu, sepet, sipariş, kupon, değerlendirme, istek listesi ve kargo süreçlerini rol tabanlı bir API üzerinden yönetir.

## Öne Çıkanlar

- JWT tabanlı kimlik doğrulama ve rol yönetimi
- Ürün, kategori, marka ve listeleme yönetimi
- Sepet, adres, sipariş, ödeme ve fatura akışları
- Satıcı başvurusu, satıcı paneli ve ürün onay süreci
- Kuponlar, değerlendirmeler, istek listesi ve görüntüleme geçmişi
- FluentValidation ile istek doğrulama

## Teknolojiler

| Katman | Teknolojiler |
| --- | --- |
| Backend | ASP.NET Core 10, Entity Framework Core, Identity, SQLite |
| Frontend | React, Vite, Zustand, React Router, Axios |

## Yerel Kurulum

### Backend

```bash
dotnet restore Backend.sln
dotnet run --project MarketBackend
```

Geliştirme ayarlarını `MarketBackend/appsettings.json`, ortam değişkenleri veya .NET User Secrets üzerinden yapılandırın. Özellikle `JwtSettings:SecretKey` değerini yerel ortamda güvenli bir anahtarla değiştirin. API adresi terminalde gösterilir.

### Frontend

```bash
cd MarketFrontend
npm install
npm run dev
```

## Klasör Yapısı

```text
MarketBackend/   API, veri modeli, doğrulayıcılar ve servisler
MarketFrontend/  React kullanıcı arayüzü
Backend.sln      .NET çözüm dosyası
```

## Not

Yerel veritabanı ve geliştirme ayarları örnek kullanım içindir. Üretimde gizli anahtarları kaynak koda eklemeyin ve CORS/JWT ayarlarını ortam değişkenleriyle yönetin.
