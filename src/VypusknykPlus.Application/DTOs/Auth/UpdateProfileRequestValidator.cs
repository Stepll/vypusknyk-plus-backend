using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Auth;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ім'я обов'язкове")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .Matches(@"^\+380\d{9}$").WithMessage("Телефон має бути у форматі +380XXXXXXXXX")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}
