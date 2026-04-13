using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Auth;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий")
            .EmailAddress().WithMessage("Невалідний email");
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Токен обов'язковий");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новий пароль обов'язковий")
            .MinimumLength(6).WithMessage("Пароль має містити щонайменше 6 символів");
    }
}
