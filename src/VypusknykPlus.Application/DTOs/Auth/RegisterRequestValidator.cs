using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий")
            .EmailAddress().WithMessage("Невалідний email")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обов'язковий")
            .MinimumLength(6).WithMessage("Пароль має містити щонайменше 6 символів");

        RuleFor(x => x.FullName)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$").WithMessage("Телефон має бути у форматі +380XXXXXXXXX")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}
