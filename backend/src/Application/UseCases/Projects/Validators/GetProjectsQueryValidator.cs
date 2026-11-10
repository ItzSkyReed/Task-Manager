using Application.UseCases.Projects.Queries;
using FluentValidation;

namespace Application.UseCases.Projects.Validators;

public sealed class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть не меньше 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен находиться в диапазоне от 1 до 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .WithMessage("Строка поиска не должна превышать 200 символов.");
    }
}