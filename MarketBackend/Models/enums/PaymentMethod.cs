namespace MarketBackend.Models.Enums;

public enum PaymentMethod
{
    CreditCard = 0,
    BankTransfer = 1,      // Havale/EFT
    CashOnDelivery = 2,    // Kapıda ödeme
    Wallet = 3,            // Cüzdan
    CardStored = 4         // Kayıtlı kart
}