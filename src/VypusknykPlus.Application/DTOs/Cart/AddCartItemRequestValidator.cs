using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Cart;

public class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Невалідний ProductId");

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
