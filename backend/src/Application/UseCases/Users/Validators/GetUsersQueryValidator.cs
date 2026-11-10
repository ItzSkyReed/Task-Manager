using Application.UseCases.Users.Queries;
using FluentValidation;

namespace Application.UseCases.Users.Validators;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть больше или равен 1.")
            .OverridePropertyName("page");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен быть в диапазоне от 1 до 100.")
            .OverridePropertyName("page_size");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Строка поиска не должна превышать 100 символов.")
            .OverridePropertyName("search");
    }
}