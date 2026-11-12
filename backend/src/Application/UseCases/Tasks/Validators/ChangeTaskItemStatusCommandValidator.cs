using Application.UseCases.Tasks.Commands;
using FluentValidation;

namespace Application.UseCases.Tasks.Validators;

public sealed class ChangeTaskItemStatusCommandValidator : AbstractValidator<ChangeTaskItemStatusCommand>
{
    public ChangeTaskItemStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Идентификатор задачи обязателен.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Указан недопустимый статус задачи.");
    }
}