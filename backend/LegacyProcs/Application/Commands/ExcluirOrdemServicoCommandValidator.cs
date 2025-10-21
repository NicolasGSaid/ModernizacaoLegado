using FluentValidation;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Validator para ExcluirOrdemServicoCommand
/// Valida ID antes da execução
/// </summary>
public class ExcluirOrdemServicoCommandValidator : AbstractValidator<ExcluirOrdemServicoCommand>
{
    public ExcluirOrdemServicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID deve ser maior que zero");
    }
}
