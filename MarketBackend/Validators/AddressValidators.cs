using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class AddressCreateDtoValidator : AbstractValidator<AddressCreateDto>
{
    public AddressCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Adres başlığı zorunludur")
            .MaximumLength(50).WithMessage("Adres başlığı en fazla 50 karakter olabilir");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Alıcı adı zorunludur")
            .MaximumLength(100).WithMessage("Alıcı adı en fazla 100 karakter olabilir");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Telefon numarası zorunludur")
            .Matches(@"^0[0-9]{10}$").WithMessage("Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır (örn: 05551234567)");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Ülke zorunludur")
            .MaximumLength(50).WithMessage("Ülke en fazla 50 karakter olabilir");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("İl seçimi zorunludur")
            .MaximumLength(50).WithMessage("İl en fazla 50 karakter olabilir");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe seçimi zorunludur")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("Mahalle seçimi zorunludur")
            .MaximumLength(100).WithMessage("Mahalle en fazla 100 karakter olabilir");

        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Adres detayı zorunludur (Sokak, Cadde, Bina No, Daire vb.)")
            .MinimumLength(10).WithMessage("Adres detayı en az 10 karakter olmalıdır")
            .MaximumLength(300).WithMessage("Adres detayı en fazla 300 karakter olabilir");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Posta kodu zorunludur")
            .Matches(@"^[0-9]{5}$").WithMessage("Posta kodu 5 haneli olmalıdır");

        RuleFor(x => x.AddressType)
            .IsInEnum().WithMessage("Geçerli bir adres türü seçiniz");
    }
}

public class AddressUpdateDtoValidator : AbstractValidator<AddressUpdateDto>
{
    public AddressUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Adres başlığı zorunludur")
            .MaximumLength(50).WithMessage("Adres başlığı en fazla 50 karakter olabilir");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Alıcı adı zorunludur")
            .MaximumLength(100).WithMessage("Alıcı adı en fazla 100 karakter olabilir");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Telefon numarası zorunludur")
            .Matches(@"^0[0-9]{10}$").WithMessage("Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır (örn: 05551234567)");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Ülke zorunludur")
            .MaximumLength(50).WithMessage("Ülke en fazla 50 karakter olabilir");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("İl seçimi zorunludur")
            .MaximumLength(50).WithMessage("İl en fazla 50 karakter olabilir");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe seçimi zorunludur")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("Mahalle seçimi zorunludur")
            .MaximumLength(100).WithMessage("Mahalle en fazla 100 karakter olabilir");

        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Adres detayı zorunludur (Sokak, Cadde, Bina No, Daire vb.)")
            .MinimumLength(10).WithMessage("Adres detayı en az 10 karakter olmalıdır")
            .MaximumLength(300).WithMessage("Adres detayı en fazla 300 karakter olabilir");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Posta kodu zorunludur")
            .Matches(@"^[0-9]{5}$").WithMessage("Posta kodu 5 haneli olmalıdır");

        RuleFor(x => x.AddressType)
            .IsInEnum().WithMessage("Geçerli bir adres türü seçiniz");
    }
}