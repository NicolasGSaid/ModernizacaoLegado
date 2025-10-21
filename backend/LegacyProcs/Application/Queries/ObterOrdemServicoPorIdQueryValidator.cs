using FluentValidation;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Validator para ObterOrdemServicoPorIdQuery
/// Valida ID antes da execução
/// </summary>
public class ObterOrdemServicoPorIdQueryValidator : AbstractValidator<ObterOrdemServicoPorIdQuery>
{
    public ObterOrdemServicoPorIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID deve ser maior que zero");
    }
}
