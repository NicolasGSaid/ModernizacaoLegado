using FluentValidation;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Validator para AlterarStatusOrdemServicoCommand
/// Valida ID e Status antes da execução
/// </summary>
public class AlterarStatusOrdemServicoCommandValidator : AbstractValidator<AlterarStatusOrdemServicoCommand>
{
    public AlterarStatusOrdemServicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID deve ser maior que zero");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status é obrigatório")
            .Must(BeValidStatus)
            .WithMessage("Status deve ser: Pendente, EmAndamento ou Concluida");
    }

    private bool BeValidStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return false;

        var validStatuses = new[] { "Pendente", "EmAndamento", "Concluida" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
