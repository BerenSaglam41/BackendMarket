namespace MarketBackend.Models.Enums;

public enum ShipmentStatus
{
    Pending = 0,             // Sipariş hazır ama kargoya verilmedi
    ReadyToShip = 1,         // Paket hazırlandı, kargoya teslim edilecek
    Shipped = 2,             // Kargo firmasına verildi
    InTransit = 3,           // Aktarma merkezleri arasında taşınıyor
    OutForDelivery = 4,      // Kurye dağıtıma çıktı
    Delivered = 5,           // Müşteriye teslim edildi
    FailedDelivery = 6,      // Teslimat başarısız (adres yok, müşteri yok)
    ReturnedToSender = 7,    // Satıcıya iade edildi
    Cancelled = 8            // Gönderi iptal edildi
}