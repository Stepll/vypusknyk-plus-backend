using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Orders;

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Невалідний ProductId")
            .When(x => x.ProductId.HasValue);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва товару обов'язкова")
            .MaximumLength(200);

        RuleFor(x => x.Qty)
            .GreaterThan(0).WithMessage("Кількість має бути більше 0");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Ціна має бути більше 0");
    }
}

public class CreateOrderDeliveryRequestValidator : AbstractValidator<CreateOrderDeliveryRequest>
{
    private static readonly string[] ValidMethods = ["nova-poshta", "ukrposhta"];

    public CreateOrderDeliveryRequestValidator()
    {
        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Метод доставки обов'язковий")
            .Must(m => ValidMethods.Contains(m))
            .WithMessage("Метод доставки має бути 'nova-poshta' або 'ukrposhta'");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Вкажіть місто")
            .MaximumLength(200)
            .When(x => x.Method == "nova-poshta");

        RuleFor(x => x.Warehouse)
            .NotEmpty().WithMessage("Вкажіть відділення або поштомат")
            .MaximumLength(200)
            .When(x => x.Method == "nova-poshta");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Вкажіть поштовий індекс")
            .Matches(@"^\d{5}$").WithMessage("Поштовий індекс має бути 5-значним")
            .When(x => x.Method == "ukrposhta");
    }
}

public class CreateOrderRecipientRequestValidator : AbstractValidator<CreateOrderRecipientRequest>
{
    public CreateOrderRecipientRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Вкажіть ПІБ")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Вкажіть номер телефону")
            .Matches(@"^\+380\d{9}$").WithMessage("Телефон має бути у форматі +380XXXXXXXXX");
    }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    private static readonly string[] ValidPayments = ["cod", "online"];

    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Замовлення має містити хоча б один товар");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());

        RuleFor(x => x.Delivery)
            .NotNull().WithMessage("Інформація про доставку обов'язкова")
            .SetValidator(new CreateOrderDeliveryRequestValidator());

        RuleFor(x => x.Recipient)
            .NotNull().WithMessage("Інформація про отримувача обов'язкова")
            .SetValidator(new CreateOrderRecipientRequestValidator());

        RuleFor(x => x.Payment)
            .NotEmpty().WithMessage("Метод оплати обов'язковий")
            .Must(p => ValidPayments.Contains(p))
            .WithMessage("Метод оплати має бути 'cod' або 'online'");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Невалідний email")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Comment)
            .MaximumLength(1000);
    }
}
