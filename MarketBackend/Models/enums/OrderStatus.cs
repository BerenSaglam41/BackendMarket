namespace MarketBackend.Models.Enums;

public enum OrderStatus
{
    AwaitingPayment = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Returned = 5
}