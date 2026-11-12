using Application.UseCases.Tasks.Commands;
using FluentValidation;

namespace Application.UseCases.Tasks.Validators;

public sealed class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    public CreateTaskItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название задачи не может быть пустым.")
            .MaximumLength(200).WithMessage("Название задачи не должно превышать 200 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Описание задачи не должно превышать 4000 символов.");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Необходимо указать проект для задачи.");

        RuleFor(x => x.AssigneeId)
            .NotEqual(Guid.Empty)
            .When(x => x.AssigneeId.HasValue)
            .WithMessage("Идентификатор исполнителя не может быть пустым Id.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Некорректное значение статуса задачи.");
    }
}