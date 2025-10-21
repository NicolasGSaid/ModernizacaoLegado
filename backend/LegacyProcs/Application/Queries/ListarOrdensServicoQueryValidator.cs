using FluentValidation;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Validator para ListarOrdensServicoQuery
/// Valida parâmetros de paginação
/// </summary>
public class ListarOrdensServicoQueryValidator : AbstractValidator<ListarOrdensServicoQuery>
{
    public ListarOrdensServicoQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("Tamanho da página deve ser no máximo 100 itens");

        RuleFor(x => x.Filtro)
            .MaximumLength(200)
            .WithMessage("Filtro deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Filtro));
    }
}
