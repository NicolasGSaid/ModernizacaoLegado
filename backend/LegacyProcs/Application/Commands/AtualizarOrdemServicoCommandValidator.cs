using FluentValidation;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Validador para AtualizarOrdemServicoCommand
/// </summary>
public class AtualizarOrdemServicoCommandValidator : AbstractValidator<AtualizarOrdemServicoCommand>
{
    public AtualizarOrdemServicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.TecnicoId)
            .GreaterThan(0).WithMessage("TecnicoId deve ser maior que zero");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(BeValidStatus).WithMessage("Status inválido. Valores aceitos: Pendente, EmAndamento, Concluída, Cancelada");
    }

    private bool BeValidStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return false;

        var validStatuses = new[] { "Pendente", "EmAndamento", "Em Andamento", "Concluída", "Concluida", "Cancelada" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
