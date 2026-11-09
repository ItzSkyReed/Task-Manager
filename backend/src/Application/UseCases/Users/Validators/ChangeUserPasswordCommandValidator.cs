using Application.UseCases.Users.Commands;
using FluentValidation;

namespace Application.UseCases.Users.Validators;

public sealed class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Текущий пароль обязателен.")
            .OverridePropertyName("current_password");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен.")
            .NotEqual(x => x.CurrentPassword) .WithMessage("Новый пароль должен отличаться от текущего.")
            .MinimumLength(8).WithMessage("Новый пароль должен быть не короче 8 символов.")
            .OverridePropertyName("new_password");
    }
}