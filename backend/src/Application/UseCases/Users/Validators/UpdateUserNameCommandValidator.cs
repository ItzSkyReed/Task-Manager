using Application.UseCases.Users.Commands;
using FluentValidation;

namespace Application.UseCases.Users.Validators;

public sealed class UpdateUserNameCommandValidator : AbstractValidator<UpdateUserNameCommand>
{
    public UpdateUserNameCommandValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage("Новое имя не может быть пустым")
            .MaximumLength(100).WithMessage("Имя не может быть длиннее 100 символов.")
            .OverridePropertyName("name");
    }
}