using MarketBackend.Models.Enums;

namespace MarketBackend.Models;

public class Shipment
{
    public int ShipmentId { get; set; }                       // PK

    // Sipariş ilişkisi
    public int OrderId { get; set; }
    public Order Order { get; set; }

    // Kargo bilgileri
    public string TrackingNumber { get; set; }
    public string ShippingProvider { get; set; }
    public ShipmentStatus ShipmentStatus { get; set; }                // Ready, Shipped, Delivered...

    // Ücret & lojistik
    public decimal? ShippingCost { get; set; }                // Paket bazında maliyet
    public decimal? Weight { get; set; }

    // Tahmini ve gerçek zamanlar
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ShippedAt { get; set; }                  // Kargoya verildiği zaman
    public DateTime? DeliveredAt { get; set; }                // Teslim edildiği zaman

    // Kargo firmasının referansı
    public string ShipperReference { get; set; }

    // Oluşturulma zamanı
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ShipmentEvent> Events { get; set; } = new List<ShipmentEvent>();

}