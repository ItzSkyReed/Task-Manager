using Application.UseCases.Users.Commands;
using FluentValidation;

namespace Application.UseCases.Users.Validators;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым.")
            .EmailAddress().WithMessage("Некорректный формат Email.")
            .MaximumLength(255).WithMessage("Email не должен превышать 255 символов.")
            .OverridePropertyName("email");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя пользователя обязательно.")
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов.")
            .OverridePropertyName("name");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов.")
            .OverridePropertyName("password");
    }
}