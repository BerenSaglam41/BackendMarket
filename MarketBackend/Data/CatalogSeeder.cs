using MarketBackend.Data;
using MarketBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Data
{
    public static class CatalogSeeder
    {
        public static async Task SeedBrandsAndCategoriesAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            var now = DateTime.UtcNow;

            // ====================================================
            // 1) CATEGORIES
            // ====================================================
            if (!await context.Categories.AnyAsync())
            {
                // ------------ ROOT CATEGORIES ------------
                var rootCategories = new List<Category>
                {
                    new Category
                    {
                        Name = "Elektronik",
                        Slug = "elektronik",
                        Description = "Telefon, bilgisayar, TV ve diğer elektronik ürünler.",
                        ImageUrl = "/images/categories/elektronik.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Elektronik Ürünler",
                        MetaDescription = "Telefon, bilgisayar, TV ve elektronik ürünler.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ev ve Yaşam",
                        Slug = "ev-ve-yasam",
                        Description = "Ev, dekorasyon, mutfak ve yaşam ürünleri.",
                        ImageUrl = "/images/categories/ev-yasam.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Ev ve Yaşam Ürünleri",
                        MetaDescription = "Ev, dekorasyon ve yaşam ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Moda",
                        Slug = "moda",
                        Description = "Kadın, erkek, çocuk giyim ve ayakkabı.",
                        ImageUrl = "/images/categories/moda.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Moda ve Giyim",
                        MetaDescription = "Kadın, erkek ve çocuk giyim ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Spor ve Outdoor",
                        Slug = "spor-ve-outdoor",
                        Description = "Spor giyim, ekipman ve outdoor ürünler.",
                        ImageUrl = "/images/categories/spor-outdoor.jpg",
                        OrderIndex = 4,
                        IsActive = true,
                        MetaTitle = "Spor ve Outdoor",
                        MetaDescription = "Spor ve outdoor ekipmanları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Kozmetik ve Kişisel Bakım",
                        Slug = "kozmetik-ve-kisisel-bakim",
                        Description = "Makyaj, cilt ve kişisel bakım ürünleri.",
                        ImageUrl = "/images/categories/kozmetik.jpg",
                        OrderIndex = 5,
                        IsActive = true,
                        MetaTitle = "Kozmetik ve Kişisel Bakım",
                        MetaDescription = "Makyaj, cilt bakımı ve kişisel bakım ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Oyun ve Konsol",
                        Slug = "oyun-ve-konsol",
                        Description = "Oyun konsolları, oyunlar ve aksesuarları.",
                        ImageUrl = "/images/categories/oyun-konsol.jpg",
                        OrderIndex = 6,
                        IsActive = true,
                        MetaTitle = "Oyun ve Konsol",
                        MetaDescription = "Oyun konsolları ve video oyunları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Kitap ve Kırtasiye",
                        Slug = "kitap-ve-kirtasiye",
                        Description = "Kitaplar, defter, kalem ve kırtasiye ürünleri.",
                        ImageUrl = "/images/categories/kitap-kirtasiye.jpg",
                        OrderIndex = 7,
                        IsActive = true,
                        MetaTitle = "Kitap ve Kırtasiye",
                        MetaDescription = "Kitap ve kırtasiye ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Anne ve Bebek",
                        Slug = "anne-ve-bebek",
                        Description = "Anne ve bebek ürünleri, bebek bezi ve tekstil.",
                        ImageUrl = "/images/categories/anne-bebek.jpg",
                        OrderIndex = 8,
                        IsActive = true,
                        MetaTitle = "Anne ve Bebek Ürünleri",
                        MetaDescription = "Bebek bezi, mama ve anne ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Otomotiv ve Yapı Market",
                        Slug = "otomotiv-ve-yapi-market",
                        Description = "Otomotiv ve yapı market ürünleri.",
                        ImageUrl = "/images/categories/oto-yapi.jpg",
                        OrderIndex = 9,
                        IsActive = true,
                        MetaTitle = "Otomotiv ve Yapı Market",
                        MetaDescription = "Oto aksesuar ve yapı market ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ofis ve Büro",
                        Slug = "ofis-ve-buro",
                        Description = "Ofis mobilyaları, ekipman ve kırtasiye.",
                        ImageUrl = "/images/categories/ofis-buro.jpg",
                        OrderIndex = 10,
                        IsActive = true,
                        MetaTitle = "Ofis ve Büro",
                        MetaDescription = "Ofis mobilya ve ofis kırtasiye ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Evcil Hayvan",
                        Slug = "evcil-hayvan",
                        Description = "Kedi, köpek ve diğer evcil hayvan ürünleri.",
                        ImageUrl = "/images/categories/evcil-hayvan.jpg",
                        OrderIndex = 11,
                        IsActive = true,
                        MetaTitle = "Evcil Hayvan Ürünleri",
                        MetaDescription = "Kedi, köpek ve evcil hayvan ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Süpermarket",
                        Slug = "supermarket",
                        Description = "Gıda, içecek ve temel tüketim ürünleri.",
                        ImageUrl = "/images/categories/supermarket.jpg",
                        OrderIndex = 12,
                        IsActive = true,
                        MetaTitle = "Süpermarket Ürünleri",
                        MetaDescription = "Gıda, içecek ve temel tüketim ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };

                await context.Categories.AddRangeAsync(rootCategories);
                await context.SaveChangesAsync();

                // Root'ları DB'den al (ID garantili)
                var elektronik = await context.Categories.SingleAsync(c => c.Slug == "elektronik");
                var evYasam   = await context.Categories.SingleAsync(c => c.Slug == "ev-ve-yasam");
                var moda      = await context.Categories.SingleAsync(c => c.Slug == "moda");
                var spor      = await context.Categories.SingleAsync(c => c.Slug == "spor-ve-outdoor");
                var kozmetik  = await context.Categories.SingleAsync(c => c.Slug == "kozmetik-ve-kisisel-bakim");
                var oyun      = await context.Categories.SingleAsync(c => c.Slug == "oyun-ve-konsol");
                var kitap     = await context.Categories.SingleAsync(c => c.Slug == "kitap-ve-kirtasiye");
                var anneBebek = await context.Categories.SingleAsync(c => c.Slug == "anne-ve-bebek");
                var otoYapi   = await context.Categories.SingleAsync(c => c.Slug == "otomotiv-ve-yapi-market");
                var ofis      = await context.Categories.SingleAsync(c => c.Slug == "ofis-ve-buro");
                var pet       = await context.Categories.SingleAsync(c => c.Slug == "evcil-hayvan");
                var market    = await context.Categories.SingleAsync(c => c.Slug == "supermarket");

                // ------------ SUB CATEGORIES (40+ toplam) ------------
                var subCategories = new List<Category>
                {
                    // ELEKTRONIK (5)
                    new Category
                    {
                        Name = "Cep Telefonu",
                        Slug = "cep-telefonu",
                        ParentCategoryId = elektronik.CategoryId,
                        Description = "Akıllı telefonlar, tuşlu telefonlar ve aksesuarlar.",
                        ImageUrl = "/images/categories/cep-telefonu.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Cep Telefonları",
                        MetaDescription = "Akıllı cep telefonu modelleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Bilgisayar ve Tablet",
                        Slug = "bilgisayar-ve-tablet",
                        ParentCategoryId = elektronik.CategoryId,
                        Description = "Laptop, masaüstü bilgisayar ve tabletler.",
                        ImageUrl = "/images/categories/bilgisayar-tablet.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Bilgisayar & Tablet",
                        MetaDescription = "Notebook, PC ve tablet ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "TV ve Ses Sistemleri",
                        Slug = "tv-ve-ses-sistemleri",
                        ParentCategoryId = elektronik.CategoryId,
                        Description = "Televizyonlar, ses sistemleri ve soundbar’lar.",
                        ImageUrl = "/images/categories/tv-ses.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "TV & Ses Sistemleri",
                        MetaDescription = "Televizyon ve ses sistemi ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Fotoğraf ve Kamera",
                        Slug = "fotograf-ve-kamera",
                        ParentCategoryId = elektronik.CategoryId,
                        Description = "Fotoğraf makineleri, kameralar ve lensler.",
                        ImageUrl = "/images/categories/fotograf-kamera.jpg",
                        OrderIndex = 4,
                        IsActive = true,
                        MetaTitle = "Fotoğraf & Kamera",
                        MetaDescription = "Fotoğraf makinesi ve kamera ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Akıllı Saat ve Bileklik",
                        Slug = "akilli-saat-ve-bileklik",
                        ParentCategoryId = elektronik.CategoryId,
                        Description = "Akıllı saat ve bileklik modelleri.",
                        ImageUrl = "/images/categories/akilli-saat.jpg",
                        OrderIndex = 5,
                        IsActive = true,
                        MetaTitle = "Akıllı Saat & Bileklik",
                        MetaDescription = "Akıllı saat ve bileklik ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // EV & YAŞAM (4)
                    new Category
                    {
                        Name = "Mutfak Ürünleri",
                        Slug = "mutfak-urunleri",
                        ParentCategoryId = evYasam.CategoryId,
                        Description = "Mutfak gereçleri ve küçük ev aletleri.",
                        ImageUrl = "/images/categories/mutfak.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Mutfak Ürünleri",
                        MetaDescription = "Mutfak gereçleri ve küçük ev aletleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Mobilya",
                        Slug = "mobilya",
                        ParentCategoryId = evYasam.CategoryId,
                        Description = "Salon, yatak odası ve ofis mobilyaları.",
                        ImageUrl = "/images/categories/mobilya.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Mobilya",
                        MetaDescription = "Salon ve ofis mobilyaları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ev Dekorasyon",
                        Slug = "ev-dekorasyon",
                        ParentCategoryId = evYasam.CategoryId,
                        Description = "Ev dekorasyon ürünleri, halı ve perde.",
                        ImageUrl = "/images/categories/ev-dekorasyon.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Ev Dekorasyon",
                        MetaDescription = "Ev dekorasyon ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ev Tekstili",
                        Slug = "ev-tekstili",
                        ParentCategoryId = evYasam.CategoryId,
                        Description = "Nevresim, yorgan, battaniye ve tekstil ürünleri.",
                        ImageUrl = "/images/categories/ev-tekstili.jpg",
                        OrderIndex = 4,
                        IsActive = true,
                        MetaTitle = "Ev Tekstili",
                        MetaDescription = "Nevresim ve ev tekstili ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // MODA (5)
                    new Category
                    {
                        Name = "Kadın Giyim",
                        Slug = "kadin-giyim",
                        ParentCategoryId = moda.CategoryId,
                        Description = "Kadın elbise, bluz, pantolon ve dış giyim.",
                        ImageUrl = "/images/categories/kadin-giyim.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Kadın Giyim",
                        MetaDescription = "Kadın giyim ve moda ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Erkek Giyim",
                        Slug = "erkek-giyim",
                        ParentCategoryId = moda.CategoryId,
                        Description = "Erkek gömlek, pantolon, t-shirt ve ceket.",
                        ImageUrl = "/images/categories/erkek-giyim.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Erkek Giyim",
                        MetaDescription = "Erkek giyim ve moda ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Çocuk Giyim",
                        Slug = "cocuk-giyim",
                        ParentCategoryId = moda.CategoryId,
                        Description = "Çocuk giyim ve ayakkabı ürünleri.",
                        ImageUrl = "/images/categories/cocuk-giyim.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Çocuk Giyim",
                        MetaDescription = "Çocuk giyim ve moda ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ayakkabı",
                        Slug = "ayakkabi",
                        ParentCategoryId = moda.CategoryId,
                        Description = "Kadın, erkek ve çocuk ayakkabıları.",
                        ImageUrl = "/images/categories/ayakkabi.jpg",
                        OrderIndex = 4,
                        IsActive = true,
                        MetaTitle = "Ayakkabı",
                        MetaDescription = "Ayakkabı ve sneaker modelleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Çanta ve Aksesuar",
                        Slug = "canta-ve-aksesuar",
                        ParentCategoryId = moda.CategoryId,
                        Description = "Çanta, cüzdan, kemer ve aksesuar ürünleri.",
                        ImageUrl = "/images/categories/canta-aksesuar.jpg",
                        OrderIndex = 5,
                        IsActive = true,
                        MetaTitle = "Çanta ve Aksesuar",
                        MetaDescription = "Çanta ve aksesuar ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // SPOR & OUTDOOR (4)
                    new Category
                    {
                        Name = "Spor Giyim",
                        Slug = "spor-giyim",
                        ParentCategoryId = spor.CategoryId,
                        Description = "Sporcu giyim, tayt, eşofman ve tişörtler.",
                        ImageUrl = "/images/categories/spor-giyim.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Spor Giyim",
                        MetaDescription = "Spor giyim ve performans ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Spor Ayakkabı",
                        Slug = "spor-ayakkabi",
                        ParentCategoryId = spor.CategoryId,
                        Description = "Koşu, antrenman ve günlük spor ayakkabılar.",
                        ImageUrl = "/images/categories/spor-ayakkabi.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Spor Ayakkabı",
                        MetaDescription = "Koşu ve spor ayakkabıları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Fitness Ekipmanları",
                        Slug = "fitness-ekipmanlari",
                        ParentCategoryId = spor.CategoryId,
                        Description = "Ağırlık, dambıl, koşu bandı ve fitness ekipmanları.",
                        ImageUrl = "/images/categories/fitness.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Fitness Ekipmanları",
                        MetaDescription = "Fitness ekipmanları ve spor aletleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Outdoor ve Kamp",
                        Slug = "outdoor-ve-kamp",
                        ParentCategoryId = spor.CategoryId,
                        Description = "Outdoor giyim, çadır, uyku tulumu ve kamp malzemeleri.",
                        ImageUrl = "/images/categories/outdoor-kamp.jpg",
                        OrderIndex = 4,
                        IsActive = true,
                        MetaTitle = "Outdoor ve Kamp",
                        MetaDescription = "Kamp malzemeleri ve outdoor ürünler.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // KOZMETIK (3)
                    new Category
                    {
                        Name = "Makyaj",
                        Slug = "makyaj",
                        ParentCategoryId = kozmetik.CategoryId,
                        Description = "Fondöten, ruj ve göz makyaj ürünleri.",
                        ImageUrl = "/images/categories/makyaj.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Makyaj Ürünleri",
                        MetaDescription = "Makyaj ürünleri ve aksesuarları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Cilt Bakımı",
                        Slug = "cilt-bakimi",
                        ParentCategoryId = kozmetik.CategoryId,
                        Description = "Nemlendirici, serum ve yüz temizleme ürünleri.",
                        ImageUrl = "/images/categories/cilt-bakimi.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Cilt Bakımı",
                        MetaDescription = "Cilt bakım ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Saç Bakımı",
                        Slug = "sac-bakimi",
                        ParentCategoryId = kozmetik.CategoryId,
                        Description = "Şampuan, saç kremi ve saç bakım ürünleri.",
                        ImageUrl = "/images/categories/sac-bakimi.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Saç Bakımı",
                        MetaDescription = "Saç bakım ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // OYUN & KONSOL (3)
                    new Category
                    {
                        Name = "Oyun Konsolları",
                        Slug = "oyun-konsollari",
                        ParentCategoryId = oyun.CategoryId,
                        Description = "PlayStation, Xbox, Nintendo Switch ve diğer konsollar.",
                        ImageUrl = "/images/categories/oyun-konsollari.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Oyun Konsolları",
                        MetaDescription = "PlayStation, Xbox ve Nintendo konsolları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Konsol Oyunları",
                        Slug = "konsol-oyunlari",
                        ParentCategoryId = oyun.CategoryId,
                        Description = "PlayStation ve Xbox oyunları.",
                        ImageUrl = "/images/categories/konsol-oyunlari.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Konsol Oyunları",
                        MetaDescription = "PlayStation ve Xbox oyunları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "PC Oyunları",
                        Slug = "pc-oyunlari",
                        ParentCategoryId = oyun.CategoryId,
                        Description = "PC oyunları ve dijital oyun kodları.",
                        ImageUrl = "/images/categories/pc-oyunlari.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "PC Oyunları",
                        MetaDescription = "PC oyunları ve kodlar.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // KITAP & KIRTASIYE (3)
                    new Category
                    {
                        Name = "Roman ve Edebiyat",
                        Slug = "roman-ve-edebiyat",
                        ParentCategoryId = kitap.CategoryId,
                        Description = "Roman, hikaye ve edebiyat kitapları.",
                        ImageUrl = "/images/categories/roman-edebiyat.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Roman ve Edebiyat",
                        MetaDescription = "Roman ve edebiyat kitapları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Çocuk Kitapları",
                        Slug = "cocuk-kitaplari",
                        ParentCategoryId = kitap.CategoryId,
                        Description = "Çocuk ve gençlik kitapları.",
                        ImageUrl = "/images/categories/cocuk-kitaplari.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Çocuk Kitapları",
                        MetaDescription = "Çocuk kitapları ve hikayeleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Kırtasiye",
                        Slug = "kirtasiye",
                        ParentCategoryId = kitap.CategoryId,
                        Description = "Defter, kalem, ofis ve okul kırtasiye ürünleri.",
                        ImageUrl = "/images/categories/kirtasiye.jpg",
                        OrderIndex = 3,
                        IsActive = true,
                        MetaTitle = "Kırtasiye Ürünleri",
                        MetaDescription = "Okul ve ofis kırtasiye ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // ANNE & BEBEK (2)
                    new Category
                    {
                        Name = "Bebek Bezi ve Islak Mendil",
                        Slug = "bebek-bezi-ve-islak-mendil",
                        ParentCategoryId = anneBebek.CategoryId,
                        Description = "Bebek bezleri ve ıslak mendil ürünleri.",
                        ImageUrl = "/images/categories/bebek-bezi.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Bebek Bezi",
                        MetaDescription = "Bebek bezi ve ıslak mendil ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Bebek Araba ve Taşıma",
                        Slug = "bebek-araba-ve-tasima",
                        ParentCategoryId = anneBebek.CategoryId,
                        Description = "Bebek arabası, mama sandalyesi ve taşıma ürünleri.",
                        ImageUrl = "/images/categories/bebek-araba.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Bebek Arabası",
                        MetaDescription = "Bebek arabası ve taşıma ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // OTOMOTIV & YAPI MARKET (2)
                    new Category
                    {
                        Name = "Araç İçi Aksesuarlar",
                        Slug = "arac-ici-aksesuarlar",
                        ParentCategoryId = otoYapi.CategoryId,
                        Description = "Araç içi organizer, telefon tutucu ve aksesuarlar.",
                        ImageUrl = "/images/categories/arac-ici-aksesuar.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Araç İçi Aksesuarlar",
                        MetaDescription = "Araç içi aksesuar ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "El Aletleri",
                        Slug = "el-aletleri",
                        ParentCategoryId = otoYapi.CategoryId,
                        Description = "Ev ve otomotiv için el aletleri ve tamir ürünleri.",
                        ImageUrl = "/images/categories/el-aletleri.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "El Aletleri",
                        MetaDescription = "El aletleri ve tamir ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // OFIS & BÜRO (2)
                    new Category
                    {
                        Name = "Ofis Mobilyaları",
                        Slug = "ofis-mobilyalari",
                        ParentCategoryId = ofis.CategoryId,
                        Description = "Ofis masası, sandalye ve depolama ürünleri.",
                        ImageUrl = "/images/categories/ofis-mobilya.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Ofis Mobilyaları",
                        MetaDescription = "Ofis mobilyaları ve çalışma masaları.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Ofis Ekipmanları",
                        Slug = "ofis-ekipmanlari",
                        ParentCategoryId = ofis.CategoryId,
                        Description = "Yazıcı, tarayıcı ve ofis makineleri.",
                        ImageUrl = "/images/categories/ofis-ekipman.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Ofis Ekipmanları",
                        MetaDescription = "Ofis cihazları ve ekipmanlar.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // EVCIL HAYVAN (2)
                    new Category
                    {
                        Name = "Kedi Ürünleri",
                        Slug = "kedi-urunleri",
                        ParentCategoryId = pet.CategoryId,
                        Description = "Kedi maması, kumu, oyuncak ve aksesuarlar.",
                        ImageUrl = "/images/categories/kedi-urunleri.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Kedi Ürünleri",
                        MetaDescription = "Kedi maması ve kedi ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Köpek Ürünleri",
                        Slug = "kopek-urunleri",
                        ParentCategoryId = pet.CategoryId,
                        Description = "Köpek maması, tasma ve oyuncaklar.",
                        ImageUrl = "/images/categories/kopek-urunleri.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Köpek Ürünleri",
                        MetaDescription = "Köpek maması ve köpek ürünleri.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    // SUPER MARKET (2)
                    new Category
                    {
                        Name = "Kahvaltılık",
                        Slug = "kahvaltilik",
                        ParentCategoryId = market.CategoryId,
                        Description = "Peynir, zeytin, reçel, yumurta ve kahvaltılık ürünler.",
                        ImageUrl = "/images/categories/kahvaltilik.jpg",
                        OrderIndex = 1,
                        IsActive = true,
                        MetaTitle = "Kahvaltılık Ürünler",
                        MetaDescription = "Kahvaltı ürünleri ve gıda.",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Category
                    {
                        Name = "Atıştırmalık",
                        Slug = "atistirmalik",
                        ParentCategoryId = market.CategoryId,
                        Description = "Çikolata, cips ve atıştırmalık ürünler.",
                        ImageUrl = "/images/categories/atistirmalik.jpg",
                        OrderIndex = 2,
                        IsActive = true,
                        MetaTitle = "Atıştırmalık Ürünler",
                        MetaDescription = "Çikolata, cips ve atıştırmalıklar.",
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };

                // Bu liste root’larla beraber 40+ kategori yapıyor
                await context.Categories.AddRangeAsync(subCategories);
                await context.SaveChangesAsync();
            }

            // ====================================================
            // 2) BRANDS (40 GLOBAL MARKA)
            // ====================================================
            if (!await context.Brands.AnyAsync())
            {
                var brands = new List<Brand>();
                int rank = 1;

                Brand B(string name, string slug, string logo, string desc, string site,
                        string country, int? year, string email, string phone, bool featured)
                    => new Brand
                    {
                        Name = name,
                        Slug = slug,
                        LogoUrl = logo,
                        Description = desc,
                        WebsiteUrl = site,
                        IsActive = true,
                        IsFeatured = featured,
                        MetaTitle = $"{name} Ürünleri",
                        MetaDescription = desc,
                        Country = country,
                        EstablishedYear = year,
                        SupportEmail = email,
                        SupportPhone = phone,
                        PriorityRank = rank++,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                brands.AddRange(new[]
                {
                    // Teknoloji devleri
                    B("Apple", "apple", "/images/brands/apple.png",
                      "Apple iPhone, iPad, Mac ve aksesuarları.",
                      "https://www.apple.com", "ABD", 1976,
                      "support@apple.com", "+1-800-275-2273", true),

                    B("Samsung", "samsung", "/images/brands/samsung.png",
                      "Samsung telefon, TV ve beyaz eşya ürünleri.",
                      "https://www.samsung.com", "Güney Kore", 1938,
                      "support@samsung.com", "+82-2-2255-0114", true),

                    B("Xiaomi", "xiaomi", "/images/brands/xiaomi.png",
                      "Xiaomi telefon, giyilebilir cihaz ve ekosistem ürünleri.",
                      "https://www.mi.com", "Çin", 2010,
                      "support@xiaomi.com", "+86-400-100-5678", true),

                    B("Huawei", "huawei", "/images/brands/huawei.png",
                      "Huawei telefon, tablet ve ağ ürünleri.",
                      "https://www.huawei.com", "Çin", 1987,
                      "support@huawei.com", "+86-755-28780808", false),

                    B("Sony", "sony", "/images/brands/sony.png",
                      "Sony TV, ses sistemleri ve PlayStation.",
                      "https://www.sony.com", "Japonya", 1946,
                      "support@sony.com", "+81-3-6748-2111", true),

                    B("LG", "lg", "/images/brands/lg.png",
                      "LG televizyon, ses ve beyaz eşya ürünleri.",
                      "https://www.lg.com", "Güney Kore", 1958,
                      "support@lg.com", "+82-2-3777-1114", false),

                    B("Dell", "dell", "/images/brands/dell.png",
                      "Dell laptop ve masaüstü bilgisayarlar.",
                      "https://www.dell.com", "ABD", 1984,
                      "support@dell.com", "+1-800-624-9897", false),

                    B("HP", "hp", "/images/brands/hp.png",
                      "HP bilgisayar, yazıcı ve ofis çözümleri.",
                      "https://www.hp.com", "ABD", 1939,
                      "support@hp.com", "+1-650-857-1501", false),

                    B("Lenovo", "lenovo", "/images/brands/lenovo.png",
                      "Lenovo laptop ve masaüstü bilgisayar ürünleri.",
                      "https://www.lenovo.com", "Çin", 1984,
                      "support@lenovo.com", "+1-855-253-6686", false),

                    B("Asus", "asus", "/images/brands/asus.png",
                      "Asus laptop, anakart ve oyuncu ekipmanları.",
                      "https://www.asus.com", "Tayvan", 1989,
                      "support@asus.com", "+886-2-2894-3447", false),

                    B("Acer", "acer", "/images/brands/acer.png",
                      "Acer laptop ve bilgisayar ürünleri.",
                      "https://www.acer.com", "Tayvan", 1976,
                      "support@acer.com", "+886-2-2696-1234", false),

                    B("Microsoft", "microsoft", "/images/brands/microsoft.png",
                      "Microsoft yazılım, Surface ve Xbox ürünleri.",
                      "https://www.microsoft.com", "ABD", 1975,
                      "support@microsoft.com", "+1-800-642-7676", true),

                    B("Google", "google", "/images/brands/google.png",
                      "Google Pixel ve teknoloji hizmetleri.",
                      "https://store.google.com", "ABD", 1998,
                      "support@google.com", "+1-650-253-0000", false),

                    B("Intel", "intel", "/images/brands/intel.png",
                      "Intel işlemciler ve donanım çözümleri.",
                      "https://www.intel.com", "ABD", 1968,
                      "support@intel.com", "+1-408-765-8080", false),

                    B("AMD", "amd", "/images/brands/amd.png",
                      "AMD işlemci ve ekran kartları.",
                      "https://www.amd.com", "ABD", 1969,
                      "support@amd.com", "+1-408-749-4000", false),

                    B("Nvidia", "nvidia", "/images/brands/nvidia.png",
                      "Nvidia ekran kartları ve yapay zeka çözümleri.",
                      "https://www.nvidia.com", "ABD", 1993,
                      "support@nvidia.com", "+1-408-486-2000", true),

                    B("Philips", "philips", "/images/brands/philips.png",
                      "Philips ev, sağlık ve kişisel bakım ürünleri.",
                      "https://www.philips.com", "Hollanda", 1891,
                      "support@philips.com", "+31-20-597-7777", false),

                    B("Bosch", "bosch", "/images/brands/bosch.png",
                      "Bosch beyaz eşya ve elektrikli aletler.",
                      "https://www.bosch.com", "Almanya", 1886,
                      "support@bosch.com", "+49-711-400-0", false),

                    B("Siemens", "siemens", "/images/brands/siemens.png",
                      "Siemens beyaz eşya ve teknolojik çözümler.",
                      "https://www.siemens.com", "Almanya", 1847,
                      "support@siemens.com", "+49-89-636-00", false),

                    B("Panasonic", "panasonic", "/images/brands/panasonic.png",
                      "Panasonic elektronik ve ev aletleri.",
                      "https://www.panasonic.com", "Japonya", 1918,
                      "support@panasonic.com", "+81-6-6908-1121", false),

                    B("Canon", "canon", "/images/brands/canon.png",
                      "Canon fotoğraf makinesi, yazıcı ve optik ürünler.",
                      "https://www.canon.com", "Japonya", 1937,
                      "support@canon.com", "+81-3-3758-2111", false),

                    B("Nikon", "nikon", "/images/brands/nikon.png",
                      "Nikon fotoğraf ve optik ürünleri.",
                      "https://www.nikon.com", "Japonya", 1917,
                      "support@nikon.com", "+81-3-6433-3600", false),

                    B("JBL", "jbl", "/images/brands/jbl.png",
                      "JBL kulaklık ve hoparlör ürünleri.",
                      "https://www.jbl.com", "ABD", 1946,
                      "support@jbl.com", "+1-800-336-4525", false),

                    B("Bose", "bose", "/images/brands/bose.png",
                      "Bose ses sistemleri ve kulaklıklar.",
                      "https://www.bose.com", "ABD", 1964,
                      "support@bose.com", "+1-800-379-2073", false),

                    B("Logitech", "logitech", "/images/brands/logitech.png",
                      "Logitech klavye, mouse ve bilgisayar çevre birimleri.",
                      "https://www.logitech.com", "İsviçre", 1981,
                      "support@logitech.com", "+41-21-863-5511", false),

                    B("Kingston", "kingston", "/images/brands/kingston.png",
                      "Kingston bellek ve depolama ürünleri.",
                      "https://www.kingston.com", "ABD", 1987,
                      "support@kingston.com", "+1-714-435-2600", false),

                    B("Western Digital", "western-digital", "/images/brands/western-digital.png",
                      "Western Digital HDD ve SSD depolama çözümleri.",
                      "https://www.westerndigital.com", "ABD", 1970,
                      "support@wdc.com", "+1-949-672-7000", false),

                    B("Seagate", "seagate", "/images/brands/seagate.png",
                      "Seagate HDD ve veri depolama ürünleri.",
                      "https://www.seagate.com", "ABD", 1979,
                      "support@seagate.com", "+1-405-324-4700", false),

                    B("SanDisk", "sandisk", "/images/brands/sandisk.png",
                      "SanDisk flash bellek ve depolama ürünleri.",
                      "https://www.sandisk.com", "ABD", 1988,
                      "support@sandisk.com", "+1-866-726-3475", false),

                    B("TP-Link", "tp-link", "/images/brands/tp-link.png",
                      "TP-Link modem, router ve ağ çözümleri.",
                      "https://www.tp-link.com", "Çin", 1996,
                      "support@tp-link.com", "+86-755-26504400", false),

                    // Moda / Giyim
                    B("Nike", "nike", "/images/brands/nike.png",
                      "Nike spor giyim, ayakkabı ve aksesuarlar.",
                      "https://www.nike.com", "ABD", 1964,
                      "support@nike.com", "+1-800-806-6453", true),

                    B("Adidas", "adidas", "/images/brands/adidas.png",
                      "Adidas spor giyim, ayakkabı ve ekipmanlar.",
                      "https://www.adidas.com", "Almanya", 1949,
                      "support@adidas.com", "+49-9132-84-0", true),

                    B("Puma", "puma", "/images/brands/puma.png",
                      "Puma spor giyim ve ayakkabılar.",
                      "https://www.puma.com", "Almanya", 1948,
                      "support@puma.com", "+49-9132-81-0", false),

                    B("Reebok", "reebok", "/images/brands/reebok.png",
                      "Reebok spor ayakkabı ve giyim ürünleri.",
                      "https://www.reebok.com", "ABD", 1958,
                      "support@reebok.com", "+1-866-870-1743", false),

                    B("Under Armour", "under-armour", "/images/brands/under-armour.png",
                      "Under Armour spor giyim ve ayakkabı.",
                      "https://www.underarmour.com", "ABD", 1996,
                      "support@underarmour.com", "+1-888-727-6687", false),

                    B("New Balance", "new-balance", "/images/brands/new-balance.png",
                      "New Balance spor ayakkabı ve giyim.",
                      "https://www.newbalance.com", "ABD", 1906,
                      "support@newbalance.com", "+1-800-595-9138", false),

                    B("H&M", "hm", "/images/brands/hm.png",
                      "H&M kadın, erkek ve çocuk giyim.",
                      "https://www.hm.com", "İsveç", 1947,
                      "support@hm.com", "+46-8-796-5500", false),

                    B("Zara", "zara", "/images/brands/zara.png",
                      "Zara moda ve giyim ürünleri.",
                      "https://www.zara.com", "İspanya", 1974,
                      "support@zara.com", "+34-981-185-400", false),

                    B("Uniqlo", "uniqlo", "/images/brands/uniqlo.png",
                      "Uniqlo günlük giyim ve basic ürünler.",
                      "https://www.uniqlo.com", "Japonya", 1949,
                      "support@uniqlo.com", "+81-3-6865-0050", false),

                    B("IKEA", "ikea", "/images/brands/ikea.png",
                      "IKEA mobilya ve ev dekorasyon ürünleri.",
                      "https://www.ikea.com", "İsveç", 1943,
                      "support@ikea.com", "+46-771-50-00-50", true)
                });

                // En az 40 brand oldu (hatta biraz fazla bile)
                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }
        }
    }
}