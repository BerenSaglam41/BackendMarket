namespace MarketBackend.Models;

public class ShipmentEvent
{
    public int ShipmentEventId { get; set; }                  // PK

    // Hangi Shipment'a bağlı olduğu
    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; }

    // Kargo durumu
    public string Status { get; set; }                        // Örn: OUT_FOR_DELIVERY, IN_TRANSIT
    public string Location { get; set; }                      // "İstanbul Hub", "İzmir Transfer"

    public DateTime EventTime { get; set; }                   // Gerçekleşme zamanı

    // Açıklama & meta bilgiler
    public string Description { get; set; }                   // Olay açıklaması
    public bool IsCustomerVisible { get; set; }               // Müşteriye gösterilsin mi?

    public string RawData { get; set; }                       // Kargo API’den gelen JSON/XML

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // Sistem kayıt zamanı
}