using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Cart;

public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Невалідний ProductId")
            .When(x => x.ProductId.HasValue);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва обов'язкова для кастомного товару")
            .When(x => !x.ProductId.HasValue);

        RuleFor(x => x.Price)
            .NotNull().GreaterThan(0).WithMessage("Ціна обов'язкова для кастомного товару")
            .When(x => !x.ProductId.HasValue);

        RuleFor(x => x.Qty)
            .GreaterThan(0).WithMessage("Кількість має бути більше 0");
    }
}

public class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.Qty)
            .GreaterThan(0).WithMessage("Кількість має бути більше 0");
    }
}
