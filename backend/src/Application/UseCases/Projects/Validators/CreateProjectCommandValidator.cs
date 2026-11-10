using Application.UseCases.Projects.Commands;
using FluentValidation;

namespace Application.UseCases.Projects.Validators;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название проекта не может быть пустым.")
            .MaximumLength(200).WithMessage("Название проекта не должно превышать 200 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Описание проекта не должно превышать 5000 символов.");
    }
}