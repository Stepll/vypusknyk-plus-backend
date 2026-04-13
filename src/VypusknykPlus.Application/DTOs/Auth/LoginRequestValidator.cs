using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий")
            .EmailAddress().WithMessage("Невалідний email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обов'язковий");
    }
}
