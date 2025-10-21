using FluentValidation;

namespace LegacyProcs.Application.Commands;

public class ExcluirTecnicoCommandValidator : AbstractValidator<ExcluirTecnicoCommand>
{
    public ExcluirTecnicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID deve ser maior que zero");
    }
}
