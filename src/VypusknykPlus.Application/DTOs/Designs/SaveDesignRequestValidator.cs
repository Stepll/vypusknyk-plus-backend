using FluentValidation;

namespace VypusknykPlus.Application.DTOs.Designs;

public class SaveDesignRequestValidator : AbstractValidator<SaveDesignRequest>
{
    public SaveDesignRequestValidator()
    {
        RuleFor(x => x.DesignName)
            .NotEmpty().WithMessage("Назва дизайну обов'язкова")
            .MaximumLength(200);

        RuleFor(x => x.State)
            .NotNull().WithMessage("Стан дизайну обов'язковий");

        RuleFor(x => x.State.MainText)
            .NotEmpty().WithMessage("Основний текст обов'язковий")
            .MaximumLength(500)
            .When(x => x.State != null);

        RuleFor(x => x.State.Color)
            .NotEmpty().WithMessage("Колір обов'язковий")
            .MaximumLength(50)
            .When(x => x.State != null);

        RuleFor(x => x.State.PrintType)
            .NotEmpty().WithMessage("Тип друку обов'язковий")
            .MaximumLength(50)
            .When(x => x.State != null);

        RuleFor(x => x.State.Material)
            .NotEmpty().WithMessage("Матеріал обов'язковий")
            .MaximumLength(50)
            .When(x => x.State != null);

        RuleFor(x => x.State.Font)
            .NotEmpty().WithMessage("Шрифт обов'язковий")
            .MaximumLength(50)
            .When(x => x.State != null);
    }
}
