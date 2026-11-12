using Application.UseCases.Tasks.Queries;
using FluentValidation;

namespace Application.UseCases.Tasks.Validators;

public sealed class GetTaskItemsQueryValidator : AbstractValidator<GetTaskItemsQuery>
{
    public GetTaskItemsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть не меньше 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен быть в диапазоне от 1 до 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .WithMessage("Строка поиска не должна превышать 200 символов.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Указан некорректный статус задачи.");
    }
}