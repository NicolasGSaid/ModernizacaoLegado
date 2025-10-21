using FluentValidation;

namespace LegacyProcs.Application.Queries;

public class ListarClientesQueryValidator : AbstractValidator<ListarClientesQuery>
{
    public ListarClientesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100).WithMessage("Tamanho da página deve ser no máximo 100 itens");

        RuleFor(x => x.Busca)
            .MaximumLength(200).WithMessage("Busca deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Busca));
    }
}
