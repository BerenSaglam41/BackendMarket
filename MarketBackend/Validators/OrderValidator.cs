using FluentValidation;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.ShippingAddressId)
            .NotEmpty().WithMessage("Shipping Address ID is required.")
            .GreaterThan(0).WithMessage("Shipping Address ID must be greater than 0.");
        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");
        
    }
}
public class OrderUpdateStatusDtoValidator : AbstractValidator<OrderUpdateStatusDto>
{
    public OrderUpdateStatusDtoValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid order status.");
        
        When(x => x.NewStatus == OrderStatus.Shipped, () =>
        {
            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("Tracking number is required when marking order as shipped.");
        });
    }
}   