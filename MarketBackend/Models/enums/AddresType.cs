namespace MarketBackend.Models.Enums;

public enum AddressType
{
    Shipping = 0,     // Teslimat adresi
    Billing = 1,      // Fatura adresi
    Both = 2          // Hem teslimat hem fatura olarak kullanılan adres
}