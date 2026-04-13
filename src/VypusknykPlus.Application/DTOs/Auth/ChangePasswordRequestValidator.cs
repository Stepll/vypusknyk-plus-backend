using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Поточний пароль обов'язковий");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новий пароль обов'язковий")
            .MinimumLength(6).WithMessage("Пароль має містити щонайменше 6 символів");
    }
}
