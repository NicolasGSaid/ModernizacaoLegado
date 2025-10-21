using FluentValidation;

namespace LegacyProcs.Application.Commands;

public class AlterarStatusTecnicoCommandValidator : AbstractValidator<AlterarStatusTecnicoCommand>
{
    public AlterarStatusTecnicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");

        RuleFor(x => x.NovoStatus)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(BeValidStatus).WithMessage("Status deve ser: Ativo, Inativo ou Suspenso");
    }

    private bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "Ativo", "Inativo", "Suspenso" };
        return validStatuses.Contains(status);
    }
}
